using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.IT.IT03.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.IT.IT03;

public static class GetApprovalHistory
{
    public record Query(int DocumentId) : IRequest<IReadOnlyList<ApprovalLogDto>>;

    public class Handler : IRequestHandler<Query, IReadOnlyList<ApprovalLogDto>>
    {
        private readonly IAppDbContext _context;

        public Handler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ApprovalLogDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var documentExists = await _context.Documents
                .AnyAsync(document => document.Id == request.DocumentId, cancellationToken);

            if (!documentExists)
            {
                throw NotFoundException.Document(request.DocumentId);
            }

            return await _context.ApprovalLogs
                .AsNoTracking()
                .Where(log => log.DocumentId == request.DocumentId)
                .OrderByDescending(log => log.ActionAt)
                .ThenByDescending(log => log.Id)
                .Select(log => new ApprovalLogDto(
                    log.Id,
                    log.DocumentId,
                    log.FromStatus!.NameTh,
                    log.ToStatus!.NameTh,
                    log.Reason,
                    log.ActionBy,
                    log.ActionAt))
                .ToListAsync(cancellationToken);
        }
    }
}
