using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> entity)
    {
        entity.HasKey(e => e.Id).HasName("orders_pkey");
        entity.ToTable("orders");

        entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()").HasColumnName("id");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.PaymentMethod)
            .HasMaxLength(50)
            .HasDefaultValueSql("'cash_on_delivery'::character varying")
            .HasColumnName("payment_method");
        entity.Property(e => e.ShippingAddress).HasColumnName("shipping_address");
        entity.Property(e => e.Status)
            .HasMaxLength(30)
            .HasDefaultValueSql("'pending'::character varying")
            .HasColumnName("status");
        entity.Property(e => e.Total).HasPrecision(10, 2).HasColumnName("total");
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.User).WithMany(p => p.Orders)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("orders_user_id_fkey");
    }
}