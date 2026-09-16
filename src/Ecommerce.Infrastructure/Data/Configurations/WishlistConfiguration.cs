using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public sealed class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> entity)
    {
        entity.HasKey(e => e.Id).HasName("wishlists_pkey");
        entity.ToTable("wishlists");
        entity.HasIndex(e => new { e.UserId, e.ProductId }, "wishlists_user_id_product_id_key").IsUnique();

        entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()").HasColumnName("id");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.ProductId).HasColumnName("product_id");
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.Product).WithMany(p => p.Wishlists)
            .HasForeignKey(d => d.ProductId)
            .HasConstraintName("wishlists_product_id_fkey");

        entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("wishlists_user_id_fkey");
    }
}