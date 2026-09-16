namespace Ecommerce.Application.DTOs.Wishlist;

public class WishlistItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public DateTime? CreatedAt { get; set; }
}