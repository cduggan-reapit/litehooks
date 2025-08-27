using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reapit.Platform.LiteHooks.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Reapit.Platform.LiteHooks.Data.Context.Configuration;

/// <summary>Database configuration instructions for the <see cref="ExampleEntity"/> type.</summary>
[ExcludeFromCodeCoverage]
public class ExampleEntityConfiguration : IEntityTypeConfiguration<ExampleEntity>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ExampleEntity> builder)
    {
        builder.ToTable("examples");

        builder.HasKey(entity => entity.Id);
        builder.HasQueryFilter(entity => entity.DateDeleted == null);

        // Name must be unique.  If an entity with a given name is soft-deleted, then the name may be re-used.
        builder.HasIndex(entity => new { entity.Name, entity.DateDeleted }).IsUnique();

        builder.HasIndex(entity => entity.DateModified);
        builder.HasIndex(entity => entity.DateCreated);
        builder.HasIndex(entity => entity.DateDeleted);

        builder.Property(entity => entity.Id).HasColumnName("id").HasMaxLength(50);
        builder.Property(entity => entity.Cursor).HasColumnName("cursor");
        builder.Property(entity => entity.DateCreated).HasColumnName("created");
        builder.Property(entity => entity.DateModified).HasColumnName("modified");
        builder.Property(entity => entity.DateDeleted).HasColumnName("deleted");

        builder.Property(entity => entity.Name).HasColumnName("name").HasMaxLength(100);
        builder.Property(entity => entity.Description).HasColumnName("description").HasMaxLength(1000);

        // Don't commit the dirty flag
        builder.Ignore(entity => entity.IsDirty);
    }
}