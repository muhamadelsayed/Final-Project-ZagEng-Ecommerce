using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> entity)
    {
        entity.HasKey(e => e.Id).HasName("order_items_pkey");
        entity.ToTable("order_items");

        entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()").HasColumnName("id");
        entity.Property(e => e.OrderId).HasColumnName("order_id");
        entity.Property(e => e.ProductId).HasColumnName("product_id");
        entity.Property(e => e.Quantity).HasColumnName("quantity");
        entity.Property(e => e.UnitPrice).HasPrecision(10, 2).HasColumnName("unit_price");

        entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
            .HasForeignKey(d => d.OrderId)
            .HasConstraintName("order_items_order_id_fkey");

        entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("order_items_product_id_fkey");
    }
}