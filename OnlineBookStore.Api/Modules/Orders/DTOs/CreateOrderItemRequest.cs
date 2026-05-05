using System.ComponentModel.DataAnnotations;

namespace OnlineBookStore.Api.Modules.Orders.DTOs
{
    public class CreateOrderItemRequest // one item in the order, which includes the BookId and Quantity properties. This class is used as part of the CreateOrderRequest to represent each item that a user wants to order.
    {
        [Range(1, int.MaxValue)]
        public int BookId { get; set; }

        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}
