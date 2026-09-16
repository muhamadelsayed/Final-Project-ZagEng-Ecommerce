using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(e => e.Id).HasName("users_pkey");
        entity.ToTable("users");
        entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

        entity.Property(e => e.Id).HasDefaultValueSql("uuid_generate_v4()").HasColumnName("id");
        entity.Property(e => e.Address).HasColumnName("address");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()").HasColumnName("created_at");
        entity.Property(e => e.Email).HasMaxLength(150).HasColumnName("email");
        entity.Property(e => e.IsVerified).HasDefaultValue(false).HasColumnName("is_verified");
        entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name");
        entity.Property(e => e.OtpCode).HasMaxLength(10).HasColumnName("otp_code");
        entity.Property(e => e.OtpExpiry).HasColumnName("otp_expiry");
        entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
        entity.Property(e => e.Role)
            .HasMaxLength(20)
            .HasDefaultValueSql("'client'::character varying")
            .HasColumnName("role");
    }
}