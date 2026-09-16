using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> entity)
    {
        entity.HasKey(e => e.Id).HasName("products_pkey");
        entity.ToTable("products");

        entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()").HasColumnName("id");
        entity.Property(e => e.CategoryId).HasColumnName("category_id");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.Description).HasColumnName("description");
        entity.Property(e => e.FeaturedImage).HasColumnName("featured_image");
        entity.Property(e => e.IsVirtual).HasDefaultValue((short)0).HasColumnName("is_virtual");
        entity.Property(e => e.Price).HasPrecision(10, 2).HasColumnName("price");
        entity.Property(e => e.Stock).HasDefaultValue(0).HasColumnName("stock");
        entity.Property(e => e.Title).HasMaxLength(200).HasColumnName("title");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("now()").HasColumnName("updated_at");

        entity.HasOne(d => d.Category).WithMany(p => p.Products)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("products_category_id_fkey");
    }
}