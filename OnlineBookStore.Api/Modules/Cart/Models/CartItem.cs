using OnlineBookStore.Api.Modules.Auth.Models;
using OnlineBookStore.Api.Modules.Books.Models;

namespace OnlineBookStore.Api.Modules.Cart.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int BookId { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;

        public Book Book { get; set; } = null!;
    }
}
