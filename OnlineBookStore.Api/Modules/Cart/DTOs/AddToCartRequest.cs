using System.ComponentModel.DataAnnotations;

namespace OnlineBookStore.Api.Modules.Cart.DTOs
{
    
    public class AddToCartRequest
    {
        [Range(1, int.MaxValue)]
        public int BookId { get; set; }

        [Range(1 , 100)]
        public int Quantity { get; set; }
    }
}
