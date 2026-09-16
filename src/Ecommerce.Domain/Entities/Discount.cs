using System;
using System.Collections.Generic;

namespace Ecommerce.Domain.Entities;

public partial class Discount
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string Type { get; set; } = null!;

    public decimal Value { get; set; }

    public DateTime ValidTo { get; set; }
}
