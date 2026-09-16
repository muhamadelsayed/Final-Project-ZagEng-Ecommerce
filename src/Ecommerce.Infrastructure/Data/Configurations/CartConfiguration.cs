using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> entity)
    {
        entity.HasKey(e => e.Id).HasName("carts_pkey");

        entity.ToTable("carts");

        entity.HasIndex(e => e.UserId, "carts_user_id_key").IsUnique();

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()")
            .HasColumnName("id");
        entity.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("updated_at");
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.User).WithOne(p => p.Cart)
            .HasForeignKey<Cart>(d => d.UserId)
            .HasConstraintName("carts_user_id_fkey");
    }
}