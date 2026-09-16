using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public sealed class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> entity)
    {
        entity.HasKey(e => e.Id).HasName("discounts_pkey");
        entity.ToTable("discounts");
        entity.HasIndex(e => e.Code, "discounts_code_key").IsUnique();

        entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()").HasColumnName("id");
        entity.Property(e => e.Code).HasMaxLength(50).HasColumnName("code");
        entity.Property(e => e.Type).HasMaxLength(20).HasColumnName("type");
        entity.Property(e => e.ValidTo).HasColumnName("valid_to");
        entity.Property(e => e.Value).HasPrecision(10, 2).HasColumnName("value");
    }
}