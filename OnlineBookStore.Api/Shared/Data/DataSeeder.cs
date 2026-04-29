using OnlineBookStore.Api.Modules.Books.Models;
namespace OnlineBookStore.Api.Shared.Data
{
    public static class DataSeeder
    {
        public static void SeedBooks(AppDbContext context)
        {
            if (context.Books.Any())
                return; // Data already seeded
            var books = new List<Book> //we created a list of Book objects to represent the initial data we want to seed into our database. Each Book object has properties that correspond to the columns in our Books table
            {
                new Book
                {
                    Title = "Clean Code",
                    Author = "Robert C. Helmy",
                    Description = "A handbook of agile software craftsmanship.",
                    Category = "Programming",
                    Price = 450,
                    StockQuantity = 10,
                    ImageUrl = "clean-code.jpg"
                },

                new Book
                {
                    Title = "The Pragmatic Programmer",
                    Author = "Aly Hunt",
                    Description = "Journey to mastery for modern developers.",
                    Category = "Programming",
                    Price = 500,
                    StockQuantity = 8,
                    ImageUrl = "pragmatic-programmer.jpg"
                },
                new Book
                {
                    Title = "Atomic Habits",
                    Author = "James Clear",
                    Description = "An easy and proven way to build good habits.",
                    Category = "Self Development",
                    Price = 300,
                    StockQuantity = 15,
                    ImageUrl = "atomic-habits.jpg"
                },
                new Book
                {
                    Title = "Deep Work",
                    Author = "Cal Newport",
                    Description = "Rules for focused success in a distracted world.",
                    Category = "Productivity",
                    Price = 320,
                    StockQuantity = 12,
                    ImageUrl = "deep-work.jpg"
                },
                new Book
                {
                    Title = "Rich Dad Poor Dad",
                    Author = "Robert Kiyosaki",
                    Description = "What the rich teach their kids about money.",
                    Category = "Finance",
                    Price = 280,
                    StockQuantity = 20,
                    ImageUrl = "richdad.jpg"
                }
            };
            context.Books.AddRange(books); //we use the AddRange method of the Books DbSet to add all the Book objects in our list to the database context. This prepares them to be inserted into the database when we call SaveChanges.
            context.SaveChanges();
        }
    }
}
