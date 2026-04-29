using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Api.Modules.Books.DTOs;
using OnlineBookStore.Api.Modules.Books.Interfaces;
using OnlineBookStore.Api.Shared.Data;

namespace OnlineBookStore.Api.Modules.Books.Services
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context; // this field will hold the instance of AppDbContext that we will use to interact with our database.
       
        public BookService(AppDbContext context) // we added a constructor that takes an AppDbContext as a parameter and assigns it to the _context field. This allows us to use the injected AppDbContext instance in our service methods to perform database operations
        {
            _context = context;
        }

        public async Task<List<BookDto>> GetAllBooksAsync()  //Query the Books table asynchronously, convert each Book record into a BookDto object, collect them into a list, and return that list.
        {
            return await _context.Books
                .Select(book => new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Description = book.Description,
                    Category = book.Category,
                    Price = book.Price,
                    StockQuantity = book.StockQuantity,
                    ImageUrl = book.ImageUrl
                })
                .ToListAsync();
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Where(book => book.Id == id)
                .Select(book => new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Description = book.Description,
                    Category = book.Category,
                    Price= book.Price,
                    StockQuantity = book.StockQuantity,
                    ImageUrl = book.ImageUrl
                })
                .FirstOrDefaultAsync();
        }
        
    }
}
