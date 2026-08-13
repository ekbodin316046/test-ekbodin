using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ApprovalLogConfiguration : IEntityTypeConfiguration<ApprovalLog>
{
    public void Configure(EntityTypeBuilder<ApprovalLog> builder)
    {
        builder.ToTable("approval_log");

        builder.HasKey(log => log.Id);

        builder.Property(log => log.Id)
            .HasColumnName("approval_log_id");

        builder.Property(log => log.DocumentId)
            .HasColumnName("document_id");

        builder.Property(log => log.FromStatusId)
            .HasColumnName("from_status_id");

        builder.Property(log => log.ToStatusId)
            .HasColumnName("to_status_id");

        builder.Property(log => log.Reason)
            .HasColumnName("reason")
            .HasMaxLength(500)
            .IsRequired();

        builder.HasAuditColumns();

        builder.HasOne(log => log.Document)
            .WithMany(document => document.ApprovalLogs)
            .HasForeignKey(log => log.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(log => log.FromStatus)
            .WithMany()
            .HasForeignKey(log => log.FromStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(log => log.ToStatus)
            .WithMany()
            .HasForeignKey(log => log.ToStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(log => log.DocumentId);
    }
}
