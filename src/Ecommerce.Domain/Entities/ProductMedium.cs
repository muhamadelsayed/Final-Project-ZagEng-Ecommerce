using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class ProductMedia
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Url { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
