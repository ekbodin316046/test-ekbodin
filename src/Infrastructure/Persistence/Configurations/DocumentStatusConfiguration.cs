using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DocumentStatusConfiguration : IEntityTypeConfiguration<DocumentStatus>
{
    public void Configure(EntityTypeBuilder<DocumentStatus> builder)
    {
        builder.ToTable("DocumentStatus");

        builder.HasKey(status => status.Id);

        // Keys must match DocumentStatusCode, so they are not generated.
        builder.Property(status => status.Id).ValueGeneratedNever();

        builder.Property(status => status.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(status => status.NameTh)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(status => status.Code).IsUnique();

        builder.HasData(
            new DocumentStatus
            {
                Id = (int)DocumentStatusCode.Pending,
                Code = "PENDING",
                NameTh = DocumentStatusCode.Pending.ToNameTh(),
            },
            new DocumentStatus
            {
                Id = (int)DocumentStatusCode.Approved,
                Code = "APPROVED",
                NameTh = DocumentStatusCode.Approved.ToNameTh(),
            },
            new DocumentStatus
            {
                Id = (int)DocumentStatusCode.Rejected,
                Code = "REJECTED",
                NameTh = DocumentStatusCode.Rejected.ToNameTh(),
            });
    }
}
