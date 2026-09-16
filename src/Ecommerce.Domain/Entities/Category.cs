using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
