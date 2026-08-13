using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Document> Documents { get; }
    DbSet<DocumentStatus> DocumentStatuses { get; }
    DbSet<ApprovalLog> ApprovalLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    bool HasActiveTransaction { get; }
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default);
}
