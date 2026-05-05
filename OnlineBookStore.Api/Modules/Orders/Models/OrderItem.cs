namespace OnlineBookStore.Api.Modules.Orders.Models
{
    public class OrderItem
    {
        public int Id { get; set; } 

        public int OrderId { get; set; } // this property represents the foreign key relationship to the Order class. It indicates which order this item belongs to

        public int BookId { get; set; }

        public string BookTitle { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get ; set; } 
    }
}
