using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.IT.IT03.Dtos;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.IT.IT03.Common;

// Approve and reject differ only in the target status, so the shared work sits
// here and each command stays a separately named entry point.
public class DocumentDecisionExecutor
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserAccessor _currentUser;

    public DocumentDecisionExecutor(IAppDbContext context, ICurrentUserAccessor currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<DecisionResultDto> ExecuteAsync(
        IReadOnlyCollection<int> documentIds,
        DocumentStatusCode toStatus,
        string reason,
        CancellationToken cancellationToken)
    {
        var distinctIds = documentIds.Distinct().ToList();

        var documents = await _context.Documents
            .Where(document => distinctIds.Contains(document.Id))
            .ToListAsync(cancellationToken);

        var missingIds = distinctIds.Except(documents.Select(document => document.Id)).ToList();
        if (missingIds.Count > 0)
        {
            throw NotFoundException.Document(missingIds[0]);
        }

        var now = DateTime.UtcNow;
        var actionBy = _currentUser.UserName;
        var program = _currentUser.ProgramCode;

        foreach (var document in documents)
        {
            // Throwing here aborts the whole batch rather than silently skipping
            // a row the caller believed it had selected.
            var log = document.ChangeStatus(toStatus, reason, actionBy, program, now);
            _context.ApprovalLogs.Add(log);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new DecisionResultDto(
            documents.Count,
            documents.Select(document => document.Id).ToList(),
            toStatus.ToNameTh());
    }
}
