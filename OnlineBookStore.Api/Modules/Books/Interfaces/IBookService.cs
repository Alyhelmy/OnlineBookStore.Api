using OnlineBookStore.Api.Modules.Books.DTOs;
namespace OnlineBookStore.Api.Modules.Books.Interfaces
{
    // Why interface? Because controller should depend on abstraction, not concrete class.Professional DI practice.

    public interface IBookService
    {
        Task<List<BookDto>> GetAllBooksAsync(); // we return a list of BookDto objects to provide a simplified and consistent representation of the book data that can be easily consumed by the API clients. This allows us to decouple the internal data model (Book entity) from the external representation (BookDto), which can help with maintainability and flexibility in our application design.
        Task<BookDto?> GetBookByIdAsync(int id); // we return a nullable BookDto to handle the case where a book with the specified ID does not exist in the database. This allows us to return null if no matching book is found, which can be useful for error handling in the calling code.
    }
}
