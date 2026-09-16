using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public decimal Total { get; set; }

    public string ShippingAddress { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}
