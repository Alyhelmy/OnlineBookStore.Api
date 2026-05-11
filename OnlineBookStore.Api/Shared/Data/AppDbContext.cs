using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Api.Modules.Books.Models;
using OnlineBookStore.Api.Modules.Auth.Models;
using OnlineBookStore.Api.Modules.Orders.Models;
using OnlineBookStore.Api.Modules.Cart.Models;

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
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>  //Without this, two users can register with the exact same email. A race condition (two simultaneous registrations) could still sneak through
        {
            entity.HasIndex(u => u.Email)
            .IsUnique();

            entity.Property(u => u.Email)
                  .HasMaxLength(256)
                  .IsRequired();

            entity.Property(u => u.FullName)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(u => u.Role)
                  .HasMaxLength(20)
                  .IsRequired();

            entity.Property(u => u.PasswordHash)
                  .HasMaxLength(60)
                  .IsRequired();
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(b => b.Title)
            .HasMaxLength(200)
            .IsRequired();

            entity.Property(b => b.Author)
              .HasMaxLength(100)
              .IsRequired();

            entity.Property(b => b.Category)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(b => b.Price)
              .HasColumnType("numeric(10,2)");

            entity.ToTable(t =>
            t.HasCheckConstraint("CK_Book_StockQuantity", "\"StockQuantity\" >= 0")); //Stock should never be negative at the DB level. this is a last line of defence even if the service layer checks it

        });

        modelBuilder.Entity<Order>(entity =>
        {

            entity.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(o => o.UserId)
                  .OnDelete(DeleteBehavior.Restrict); //OnDelete.Restrict = can't delete a User who has Orders

            entity.HasMany(o => o.OrderItems)
                  .WithOne(oi => oi.Order)
                  .HasForeignKey(oi => oi.OrderId)
                  .OnDelete(DeleteBehavior.Cascade); //automatically deletes the items when the Order is deleted


            entity.Property(o => o.TotalAmount)
            .HasColumnType("numeric(10,2)");

            entity.Property(o => o.Status)
                  .HasConversion<string>()  //Storing it as a string  means you see "Pending", "Shipped" etc. directly in PostgreSQL.
                  .HasMaxLength(20);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(oi => oi.UnitPrice)
            .HasColumnType("numeric(10,2)");

            entity.Property(oi => oi.TotalPrice)
                  .HasColumnType("numeric(10,2)");

            entity.Property(oi => oi.BookTitle)
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(oi => oi.Author)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.ToTable(t =>
                t.HasCheckConstraint("CK_OrderItem_Quantity", "\"Quantity\" > 0"));
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasIndex(ci => new { ci.UserId, ci.BookId }) //A user should never have the same book in their cart twice as two separate rows
                  .IsUnique();

            entity.HasOne(ci => ci.User)
                  .WithMany()
                  .HasForeignKey(ci => ci.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ci => ci.Book)
                  .WithMany()
                  .HasForeignKey(ci => ci.BookId)
                  .OnDelete(DeleteBehavior.Cascade); //if a Book is deleted, remove it from all carts.

        });


    }


}