namespace OnlineBookStore.Api.Modules.Books.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Author { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty; // we assigned a default value to avoid null reference issues

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}