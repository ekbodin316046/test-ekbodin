using Application.Common.Interfaces;
using Application.Features.IT.IT03.Dtos;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.IT.IT03;

public static class GetDocumentList
{
    public record Query : IRequest<IReadOnlyList<DocumentListItemDto>>;

    public class Handler : IRequestHandler<Query, IReadOnlyList<DocumentListItemDto>>
    {
        private readonly IAppDbContext _context;

        public Handler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<DocumentListItemDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            return await _context.Documents
                .AsNoTracking()
                .OrderBy(document => document.Id)
                .Select(document => new DocumentListItemDto(
                    document.Id,
                    document.DocumentName,
                    document.Reason,
                    document.StatusId,
                    document.Status!.Code,
                    document.Status!.NameTh,
                    document.StatusId == (int)DocumentStatusCode.Pending,
                    document.CreatedDate,
                    document.UpdatedDate))
                .ToListAsync(cancellationToken);
        }
    }
}
