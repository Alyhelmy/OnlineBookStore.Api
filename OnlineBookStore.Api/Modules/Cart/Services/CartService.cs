using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Api.Modules.Cart.DTOs;
using OnlineBookStore.Api.Modules.Cart.Interfaces;
using OnlineBookStore.Api.Modules.Cart.Models;
using OnlineBookStore.Api.Shared.Data;
using OnlineBookStore.Api.Shared.Helpers;

namespace OnlineBookStore.Api.Modules.Cart.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CartService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        private ServiceResult<CartResponse> BuildCartResponse(
            List<CartItem> cartItems)
        {
            var response = new CartResponse();

            response.Items = cartItems.Select(cartItem => new CartItemResponse
            {
                Id = cartItem.Id,
                BookId = cartItem.BookId,
                Title = cartItem.Book.Title,
                Price = cartItem.Book.Price,
                ImageUrl = cartItem.Book.ImageUrl,
                Quantity = cartItem.Quantity,
                StockQuantity = cartItem.Book.StockQuantity,
                Subtotal = cartItem.Book.Price * cartItem.Quantity,

            }).ToList();

            response.TotalPrice = response.Items.Sum(item => item.Subtotal);

            return ServiceResult<CartResponse>.Success(response, "Cart Retrieved Successfully");
        }


        public async Task<ServiceResult<CartResponse>> GetCartAsync()
        {
            var userId = _currentUserService.UserId;

            if (userId == null)
            {
                return ServiceResult<CartResponse>.Failure("User is not authenticated");
            }

            var cartItems = await _context.CartItems
                .Include(cartItem => cartItem.Book)
                .Where(cartItem => cartItem.UserId == userId)
                .ToListAsync();

            return BuildCartResponse(cartItems);
        }
        
        public async Task<ServiceResult<CartResponse>> AddToCartAsync(AddToCartRequest request) {

            var userId = _currentUserService.UserId;

            if (userId == null)
            {
                return ServiceResult<CartResponse>.Failure("User is not authenticated");
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(book => book.Id == request.BookId);

            if (book == null)
            {
                return ServiceResult<CartResponse>.Failure($"{request.BookId} was not found");
            }

            var existingCartItem = await _context.CartItems //queries the database to find a cart item that belongs to that specific user and matches the specific book sent in the request.
                .FirstOrDefaultAsync(cartItem =>
                cartItem.UserId == userId &&
                cartItem.BookId == request.BookId);

            if (existingCartItem != null) {

                var newQuantity = existingCartItem.Quantity + request.Quantity;

                if(newQuantity > book.StockQuantity) {
                    return ServiceResult<CartResponse>.Failure("Not enough stock avaiable");
                    }

                existingCartItem.Quantity = newQuantity;

            }else{

                if (request.Quantity > book.StockQuantity)
                {
                    return ServiceResult<CartResponse>.Failure("Not enough stock available");
                }


                var cartItem = new CartItem
                {
                    UserId = userId.Value, //used .Value to unwrap int? to a plain int to match CartItem.UserId
                    BookId = request.BookId,
                    Quantity = request.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                _context.CartItems.Add(cartItem);
            }
            await _context.SaveChangesAsync();
            return await GetCartAsync();
        }

        public async Task<ServiceResult<CartResponse>> UpdateCartItemAsync (int cartItemId, UpdateCartItemRequest request)
        {
            var userId = _currentUserService.UserId;

            if (userId == null)
            {
                return ServiceResult<CartResponse>.Failure("User is not authenticated");
            }

            var cartItem = await _context.CartItems
                .Include(cartItem => cartItem.Book)
                .FirstOrDefaultAsync(cartItem =>
                cartItem.Id == cartItemId &&
                cartItem.UserId == userId);

            if (cartItem == null)
            {
                return ServiceResult<CartResponse>.Failure($"Cart item with Id {cartItemId} was not found.");
            }

            if (request.Quantity > cartItem.Book.StockQuantity)
            {
                return ServiceResult<CartResponse>.Failure("Not enough stock available");
            }

            cartItem.Quantity = request.Quantity;

            await _context.SaveChangesAsync();

            return await GetCartAsync();
        }

        public async Task<ServiceResult<bool>> RemoveCartItemAsync(int cartItemId) {

            var userId = _currentUserService.UserId;

            if (userId == null)
                return ServiceResult<bool>.Failure("User is not authenticated");

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(cartItem =>
                cartItem.Id == cartItemId &&
                cartItem.UserId == userId);

            if (cartItem == null)
            {
                return ServiceResult<bool>
                    .Failure($"Cart item with Id {cartItemId} was not found.");
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Cart item removed Successfully");
        }

        public async Task<ServiceResult<bool>> ClearCartAsync()
        {
            var userId = _currentUserService.UserId;

            if (userId == null)
                return ServiceResult<bool>.Failure("User is not authenticated");

            var cartItems = await _context.CartItems
                .Where(CartItem => CartItem.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true, "Cart cleared Successfully");
        }

    }
}
