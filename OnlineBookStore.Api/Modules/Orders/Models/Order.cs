using OnlineBookStore.Api.Shared.Enums;

namespace OnlineBookStore.Api.Modules.Orders.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending; // we set the default value of the Status property to OrderStatus.Pending. This means that when a new Order object is created, if the Status property is not explicitly set, it will automatically be initialized to OrderStatus.Pending. This is useful for ensuring that new orders start with a consistent initial status without requiring the caller to set it manually.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
