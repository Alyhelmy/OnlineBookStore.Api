using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Api.Modules.Books.DTOs;
using OnlineBookStore.Api.Modules.Books.Interfaces;
using OnlineBookStore.Api.Modules.Books.Models;
using OnlineBookStore.Api.Shared.Data;
using OnlineBookStore.Api.Shared.Helpers;

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

        public async Task<ServiceResult<BookDto>> CreateBookAsync(CreateBookRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return ServiceResult<BookDto>.Failure("Book title is required.");

            if (request.Price <= 0)
                return ServiceResult<BookDto>.Failure("Book price must be greater than zero.");

            if (request.StockQuantity < 0)
                return ServiceResult<BookDto>.Failure("Stock quantity cannot be negative.");

            var book = new Book
            {
                Title = request.Title,
                Author = request.Author,
                Description = request.Description,
                Category = request.Category,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                ImageUrl = request.ImageUrl
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var response = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                Category = book.Category,
                Price = book.Price,
                StockQuantity = book.StockQuantity,
                ImageUrl = book.ImageUrl
            };

            return ServiceResult<BookDto>.Success(response, "Book created successfully.");
        }

        public async Task<ServiceResult<BookDto>> UpdateBookAsync(int id, UpdateBookRequest request)
        {
            var book = await _context.Books.FirstOrDefaultAsync(book => book.Id == id);

            if (book == null)
                return ServiceResult<BookDto>.Failure($"Book with Id {id} was not found.");

            if (string.IsNullOrWhiteSpace(request.Title))
                return ServiceResult<BookDto>.Failure("Book title is required.");

            if (request.Price <= 0)
                return ServiceResult<BookDto>.Failure("Book price must be greater than zero.");

            if (request.StockQuantity < 0)
                return ServiceResult<BookDto>.Failure("Stock quantity cannot be negative.");

            book.Title = request.Title;
            book.Author = request.Author;
            book.Description = request.Description;
            book.Category = request.Category;
            book.Price = request.Price;
            book.StockQuantity = request.StockQuantity;
            book.ImageUrl = request.ImageUrl;

            await _context.SaveChangesAsync();

            var response = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                Category = book.Category,
                Price = book.Price,
                StockQuantity = book.StockQuantity,
                ImageUrl = book.ImageUrl
            };

            return ServiceResult<BookDto>.Success(response, "Book updated successfully.");
        }

        public async Task<ServiceResult<bool>> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FirstOrDefaultAsync(book => book.Id == id);

            if (book == null)
                return ServiceResult<bool>.Failure($"Book with Id {id} was not found.");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Book deleted successfully.");
        }

    }
}
