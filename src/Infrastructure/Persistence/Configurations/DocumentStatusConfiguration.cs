using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DocumentStatusConfiguration : IEntityTypeConfiguration<DocumentStatus>
{
    // HasData rows land in the migration, so the stamp cannot come from the
    // clock or it would change the migration on every scaffold.
    private static readonly DateTime SeedDate = new(2026, 8, 1, 9, 0, 0, DateTimeKind.Utc);
    private const string SeedUser = "SYSTEM";
    private const string SeedProgram = "SEED";

    public void Configure(EntityTypeBuilder<DocumentStatus> builder)
    {
        builder.ToTable("document_status");

        builder.HasKey(status => status.Id);

        // Keys must match DocumentStatusCode, so they are not generated.
        builder.Property(status => status.Id)
            .HasColumnName("status_id")
            .ValueGeneratedNever();

        builder.Property(status => status.Code)
            .HasColumnName("status_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(status => status.NameTh)
            .HasColumnName("status_name_tha")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasAuditColumns();

        builder.HasIndex(status => status.Code).IsUnique();

        builder.HasData(
            Row(DocumentStatusCode.Pending, "PENDING"),
            Row(DocumentStatusCode.Approved, "APPROVED"),
            Row(DocumentStatusCode.Rejected, "REJECTED"));
    }

    private static DocumentStatus Row(DocumentStatusCode code, string statusCode) => new()
    {
        Id = (int)code,
        Code = statusCode,
        NameTh = code.ToNameTh(),
        CreatedBy = SeedUser,
        CreatedDate = SeedDate,
        CreatedProgram = SeedProgram,
        UpdatedBy = SeedUser,
        UpdatedDate = SeedDate,
        UpdatedProgram = SeedProgram,
    };
}
