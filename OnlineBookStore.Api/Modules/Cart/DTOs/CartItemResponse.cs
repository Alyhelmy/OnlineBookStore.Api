namespace OnlineBookStore.Api.Modules.Cart.DTOs
{
    public class CartItemResponse
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public string Title { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public int StockQuantity { get; set; }

        public decimal Subtotal { get; set; }
    }
}
