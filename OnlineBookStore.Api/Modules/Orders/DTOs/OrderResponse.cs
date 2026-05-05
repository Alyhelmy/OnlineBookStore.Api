namespace OnlineBookStore.Api.Modules.Orders.DTOs
{
    public class OrderResponse
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string UserFullName { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public List<OrderItemResponse> Items { get; set; } = new();
    }
}
