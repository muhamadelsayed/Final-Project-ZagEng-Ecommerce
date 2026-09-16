using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> entity)
    {
        entity.HasKey(e => e.Id).HasName("cart_items_pkey");
        entity.ToTable("cart_items");
        entity.HasIndex(e => new { e.CartId, e.ProductId }, "cart_items_cart_id_product_id_key").IsUnique();

        entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()").HasColumnName("id");
        entity.Property(e => e.CartId).HasColumnName("cart_id");
        entity.Property(e => e.ProductId).HasColumnName("product_id");
        entity.Property(e => e.Quantity).HasDefaultValue(1).HasColumnName("quantity");

        entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
            .HasForeignKey(d => d.CartId)
            .HasConstraintName("cart_items_cart_id_fkey");

        entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
            .HasForeignKey(d => d.ProductId)
            .HasConstraintName("cart_items_product_id_fkey");
    }
}