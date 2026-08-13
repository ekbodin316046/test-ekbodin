using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ApprovalLogConfiguration : IEntityTypeConfiguration<ApprovalLog>
{
    public void Configure(EntityTypeBuilder<ApprovalLog> builder)
    {
        builder.ToTable("ApprovalLog");

        builder.HasKey(log => log.Id);

        builder.Property(log => log.Reason)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(log => log.ActionBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(log => log.ActionAt).IsRequired();

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
