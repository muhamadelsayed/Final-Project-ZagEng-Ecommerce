using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Address { get; set; }

    public string? Role { get; set; }

    public bool? IsVerified { get; set; }

    public string? OtpCode { get; set; }

    public DateTime? OtpExpiry { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
