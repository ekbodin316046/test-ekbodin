using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal static class AuditColumnConfiguration
{
    public static EntityTypeBuilder<TEntity> HasAuditColumns<TEntity>(
        this EntityTypeBuilder<TEntity> builder)
        where TEntity : AuditableEntity
    {
        builder.Property(entity => entity.CreatedBy)
            .HasColumnName("created_by").HasMaxLength(50).IsRequired();

        builder.Property(entity => entity.CreatedDate)
            .HasColumnName("created_date").IsRequired();

        builder.Property(entity => entity.CreatedProgram)
            .HasColumnName("created_program").HasMaxLength(50).IsRequired();

        builder.Property(entity => entity.UpdatedBy)
            .HasColumnName("updated_by").HasMaxLength(50).IsRequired();

        builder.Property(entity => entity.UpdatedDate)
            .HasColumnName("updated_date").IsRequired();

        builder.Property(entity => entity.UpdatedProgram)
            .HasColumnName("updated_program").HasMaxLength(50).IsRequired();

        return builder;
    }
}
