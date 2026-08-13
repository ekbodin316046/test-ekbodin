using Application.Common.Interfaces;
using Application.Features.IT.IT03.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.IT.IT03;

public static class GetStatusList
{
    public record Query : IRequest<IReadOnlyList<DocumentStatusDto>>;

    public class Handler : IRequestHandler<Query, IReadOnlyList<DocumentStatusDto>>
    {
        private readonly IAppDbContext _context;

        public Handler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<DocumentStatusDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            return await _context.DocumentStatuses
                .AsNoTracking()
                .OrderBy(status => status.Id)
                .Select(status => new DocumentStatusDto(
                    status.Id,
                    status.Code,
                    status.NameTh,
                    _context.Documents.Count(document => document.StatusId == status.Id)))
                .ToListAsync(cancellationToken);
        }
    }
}
