using System.ComponentModel.DataAnnotations;

namespace OnlineBookStore.Api.Modules.Cart.DTOs
{
    public class UpdateCartItemRequest
    {
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
