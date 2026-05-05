using System.ComponentModel.DataAnnotations;

namespace OnlineBookStore.Api.Modules.Orders.DTOs
{
    public class CreateOrderRequest  // whole checkout request containing all cart items , better for scalability , as we can easily add more properties to this class in the future (like shipping address, payment method, etc.) without changing the structure of the request.
    {
        [Required]
        [MinLength(1)]
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}
