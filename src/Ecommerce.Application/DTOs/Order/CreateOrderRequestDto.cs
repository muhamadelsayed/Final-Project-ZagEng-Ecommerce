namespace Ecommerce.Application.DTOs.Order;

public class CreateOrderRequestDto
{
    public string ShippingAddress { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = "CashOnDelivery";
}