using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public sealed class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> entity)
    {
        entity.HasKey(e => e.Id).HasName("product_media_pkey");
        entity.ToTable("product_media");

        entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()").HasColumnName("id");
        entity.Property(e => e.ProductId).HasColumnName("product_id");
        entity.Property(e => e.Url).HasColumnName("url");

        entity.HasOne(d => d.Product).WithMany(p => p.ProductMedia)
            .HasForeignKey(d => d.ProductId)
            .HasConstraintName("product_media_product_id_fkey");
    }
}