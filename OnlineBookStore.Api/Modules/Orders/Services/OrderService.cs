using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Api.Modules.Orders.DTOs;
using OnlineBookStore.Api.Modules.Orders.Interfaces;
using OnlineBookStore.Api.Modules.Orders.Models;
using OnlineBookStore.Api.Modules.Cart.Models;
using OnlineBookStore.Api.Shared.Data;
using OnlineBookStore.Api.Shared.Helpers;
using OnlineBookStore.Api.Shared.Enums;
using OnlineBookStore.Api.Modules.Cart.DTOs;

namespace OnlineBookStore.Api.Modules.Orders.Services
{
    public class OrderService : IOrderService
    {

        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public OrderService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ServiceResult<OrderResponse>> CreateOrderAsync()
        {
            var userId = _currentUserService.UserId;

            using var transaction = await _context.Database
                .BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead);
            try
            {

                var cartItems = await _context.CartItems
                    .Include(cartItem => cartItem.Book)
                    .Where(cartItem => cartItem.UserId == userId)
                    .ToListAsync();

                if (cartItems.Count == 0)
                {
                    return ServiceResult<OrderResponse>.Failure("Cart is empty");
                }

                foreach (var cartItem in cartItems)
                {
                    if (cartItem.Quantity > cartItem.Book.StockQuantity)
                    {
                        return ServiceResult<OrderResponse>.Failure($"Not enough stock for book: {cartItem.Book.Title}");
                    }
                }

                decimal totalPrice = cartItems.Sum(cartItem =>
                cartItem.Book.Price * cartItem.Quantity);

                var order = new Order
                {
                    UserId = userId.Value,
                    Status = OrderStatus.Pending,
                    TotalAmount = totalPrice,
                    CreatedAt = DateTime.UtcNow,
                    OrderItems = cartItems.Select(cartItem => new OrderItem
                    {
                        BookId = cartItem.BookId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Book.Price,
                        TotalPrice = cartItem.Book.Price * cartItem.Quantity
                    }).ToList()
                };

                foreach (var cartItem in cartItems)
                {
                    cartItem.Book.StockQuantity -= cartItem.Quantity;
                }

                _context.Orders.Add(order);
                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();


                var response = new OrderResponse
                {
                    Id = order.Id,
                    Status = order.Status.ToString(),
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt,
                    Items = order.OrderItems.Select(orderItem => new OrderItemResponse
                    {
                        BookId = orderItem.BookId,
                        Quantity = orderItem.Quantity,
                        UnitPrice = orderItem.UnitPrice,
                        TotalPrice = orderItem.TotalPrice
                    }).ToList()
                };

                return ServiceResult<OrderResponse>
                    .Success(response, "Order created successfully.");

            }
            catch (Exception ex) 
            {
                await transaction.RollbackAsync();
                return ServiceResult<OrderResponse>.Failure("An error occurred while creating your order.");
            }
        }

        public async Task<ServiceResult<List<OrderResponse>>> GetMyOrdersAsync()
        {
            var userId = _currentUserService.UserId;

            if (userId == null)
                return ServiceResult<List<OrderResponse>>.Failure("User not authenticated.");


            var orders = await _context.Orders   //we are querying the orders from the database based on the user id and including the order items and then projecting it to the response model
                .Where(order => order.UserId == userId.Value)
                .Include(order => order.OrderItems) // eager loading of order items to avoid multiple database calls when we access order.
                .OrderByDescending(order => order.CreatedAt)
                .Select(order => new OrderResponse 
                {
                    Id = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status.ToString(),
                    CreatedAt = order.CreatedAt,
                    Items = order.OrderItems.Select(item => new OrderItemResponse  
                    {
                        BookId = item.BookId,
                        BookTitle = item.BookTitle,
                        Author = item.Author,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice
                    }).ToList() 

                })
                .ToListAsync(); // tolistasync is used to execute the query and get the results as a list asynchronously

            return ServiceResult<List<OrderResponse>>.Success(orders, "Orders retrieved successfully.");
        }


        public async Task<ServiceResult<OrderResponse>> UpdateOrderStatusAsync(
            int orderId,
            UpdateOrderStatusRequest request)
        {
            var order = await _context.Orders
                .Include(order => order.OrderItems)
                .FirstOrDefaultAsync(order => order.Id == orderId);

            if (order == null)
                return ServiceResult<OrderResponse>.Failure($"Order with id {orderId} was not found");

            var isValidStatus = Enum.TryParse<Shared.Enums.OrderStatus>( //isValidStatus is a boolean variable that will be true if the parsing was successful and false if it was not. we are trying to parse the status from the request to the OrderStatus enum and ignoring the case sensitivity.
                request.Status,
                ignoreCase: true,
                out var newStatus); //here we are using the out keyword to declare a new variable called newStatus that will hold the parsed value of the status if the parsing was successful.

            if (!isValidStatus)
                return ServiceResult<OrderResponse>.Failure("Invalid order Status.");

            order.Status = newStatus; //if the parsing was successful we are updating the order status with the new status


            await _context.SaveChangesAsync();

            var response = new OrderResponse
            {
                Id = order.Id,  
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(item => new OrderItemResponse
                {
                    BookId = item.BookId,
                    BookTitle = item.BookTitle,
                    Author = item.Author,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                }).ToList()

            };

            return ServiceResult<OrderResponse>.Success(response, "Order status updated successfully.");

        }

        public async Task<ServiceResult<List<OrderResponse>>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(orders => orders.OrderItems)
                .OrderByDescending(order => order.CreatedAt)
                .Select( order => new OrderResponse
                {
                    Id = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status.ToString(),
                    CreatedAt = order.CreatedAt,

                    UserFullName = _context.Users
                    .Where(user => user.Id == order.UserId)
                    .Select(user => user.FullName)
                    .FirstOrDefault() ?? "Unknown User",

                    UserEmail = _context.Users
                    .Where (user => user.Id == order.UserId)
                    .Select(user => user.Email)
                    .FirstOrDefault() ?? "Unknown Email",


                    Items = order.OrderItems.Select ( item => new OrderItemResponse
                    {
                        BookId = item.BookId,
                        BookTitle = item.BookTitle,
                        Author = item.Author,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice


                    }).ToList()
                })
                .ToListAsync();

            return ServiceResult<List<OrderResponse>>.Success(orders, "Orders retrieved successfully.");
        }
    }
}
