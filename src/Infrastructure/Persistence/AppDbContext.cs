using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    private readonly ICurrentUserAccessor _currentUser;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserAccessor currentUser)
        : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentStatus> DocumentStatuses => Set<DocumentStatus>();
    public DbSet<ApprovalLog> ApprovalLogs => Set<ApprovalLog>();

    public bool HasActiveTransaction => Database.CurrentTransaction is not null;

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        Database.BeginTransactionAsync(cancellationToken);

    public Task CommitTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default) =>
        transaction.CommitAsync(cancellationToken);

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAuditColumns();

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    // The audit columns are NOT NULL, so nothing may reach the database without
    // them. An inserted row that already carries a stamp keeps it, because the
    // seeder backdates its history on purpose; an update always takes the
    // current stamp regardless of what the caller set.
    private void StampAuditColumns()
    {
        var now = DateTime.UtcNow;
        var user = _currentUser.UserName;
        var program = _currentUser.ProgramCode;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added && string.IsNullOrEmpty(entry.Entity.CreatedBy))
            {
                entry.Entity.StampCreated(user, program, now);
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.StampUpdated(user, program, now);
            }
        }
    }
}
