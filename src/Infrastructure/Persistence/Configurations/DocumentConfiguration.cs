using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(document => document.Id);

        builder.Property(document => document.DocumentName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(document => document.Reason)
            .HasMaxLength(500);

        builder.Property(document => document.CreatedAt).IsRequired();
        builder.Property(document => document.UpdatedAt).IsRequired();

        builder.HasOne(document => document.Status)
            .WithMany()
            .HasForeignKey(document => document.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(document => document.StatusId);

        builder.Ignore(document => document.IsPending);
    }
}
