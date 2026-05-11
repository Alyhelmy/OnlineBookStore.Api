namespace OnlineBookStore.Api.Modules.Cart.DTOs
{
    public class CartResponse
    {
        public List<CartItemResponse> Items { get; set; } = [];

        public decimal TotalPrice { get; set; }
    }
}
