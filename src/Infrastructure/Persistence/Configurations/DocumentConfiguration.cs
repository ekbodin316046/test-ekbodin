using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");

        builder.HasKey(document => document.Id);

        builder.Property(document => document.Id)
            .HasColumnName("document_id");

        builder.Property(document => document.DocumentName)
            .HasColumnName("document_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(document => document.Reason)
            .HasColumnName("reason")
            .HasMaxLength(500);

        builder.Property(document => document.StatusId)
            .HasColumnName("status_id");

        builder.HasAuditColumns();

        builder.HasOne(document => document.Status)
            .WithMany()
            .HasForeignKey(document => document.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(document => document.StatusId);

        builder.Ignore(document => document.IsPending);
    }
}
