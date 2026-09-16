namespace Ecommerce.Application.DTOs.Cart;

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class CartDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
}

public class AddToCartRequestDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}