using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Api.Modules.Books.Interfaces;

namespace OnlineBookStore.Api.Modules.Books.Controllers
{
    [ApiController] // This attribute indicates that this class is an API controller, which means it will handle HTTP requests and return HTTP responses.
    [Route("api/[controller]")] // This attribute defines the route template for the controller. The [controller] token will be replaced with the name of the controller, which in this case is "Books". So, the base route for this controller will be "api/books".
    public class BooksController : ControllerBase
    {
         private readonly IBookService _bookService; // we declare a private readonly field of type IBookService to hold the reference to the book service that will be injected through the constructor. This allows us to use the book service methods in our controller actions to perform operations related to books.
         
        public BooksController(IBookService bookService) // we added a constructor that takes an IBookService as a parameter and assigns it to the _bookService field. This allows us to use the injected book service instance in our controller actions to perform operations related to books.
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks() // we added an action method named GetAllBooks that returns an IActionResult. This method will handle HTTP GET requests to the "api/books" endpoint and return a list of all books in the database.
        {
            var books = await _bookService.GetAllBooksAsync(); // we call the GetAllBooksAsync method of the injected book service to retrieve a list of all books from the database asynchronously.
            return Ok(books); // we return an HTTP 200 OK response with the list of books as the response body.
        }

        [HttpGet("{id}")] // we added an action method named GetBookById that takes an integer id as a parameter. This method will handle HTTP GET requests to the "api/books/{id}" endpoint, where {id} is a placeholder for the book ID.
        public async Task<IActionResult> GetBookById(int id)
        { 
            var book = await _bookService.GetBookByIdAsync(id);

            if (book == null)
                return NotFound($"Book with ID {id} was not found. :(");

            return Ok(book);
        }
    }
}
