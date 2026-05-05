namespace OnlineBookStore.Api.Modules.Orders.DTOs
{
    public class OrderItemResponse
    {
        public int BookId { get; set; }

        public string BookTitle { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }


    }
}
