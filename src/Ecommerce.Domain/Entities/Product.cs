using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class Product
{
    public Guid Id { get; set; }

    public Guid? CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int? Stock { get; set; }

    public short? IsVirtual { get; set; }

    public string FeaturedImage { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductMedia> ProductMedia { get; set; } = new List<ProductMedia>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
