using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Api.Modules.Books.Models;
using OnlineBookStore.Api.Modules.Auth.Models;
using OnlineBookStore.Api.Modules.Orders.Models;
namespace OnlineBookStore.Api.Shared.Data;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) //we added a constructor that takes DbContextOptions<AppDbContext> as a parameter and passes it to the base DbContext class. This allows us to configure our database connection and other options when we set up our application.
        : base(options)
        {
        }

    public DbSet<Book> Books { get; set; } //we added this DbSet to represent the Books table in our database. This allows us to perform CRUD operations on the Books table using Entity Framework Core's LINQ queries and other features.
    public DbSet<User> Users { get; set; } 
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}