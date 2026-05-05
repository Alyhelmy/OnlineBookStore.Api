using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Api.Modules.Orders.DTOs;
using OnlineBookStore.Api.Modules.Orders.Interfaces;
using OnlineBookStore.Api.Modules.Orders.Models;
using OnlineBookStore.Api.Shared.Data;
using OnlineBookStore.Api.Shared.Helpers;

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

        public async Task<ServiceResult<OrderResponse>> CreateOrderAsync(CreateOrderRequest request)
        {
            var userId = _currentUserService.UserId;

            if (userId == null)
                return ServiceResult<OrderResponse>.Failure("User not authenticated.");

            if (request.Items == null || !request.Items.Any())
                return ServiceResult<OrderResponse>.Failure("Order must contain at least one item.");

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in request.Items)
            {
                var book = await _context.Books
                    .FirstOrDefaultAsync(b => b.Id == item.BookId);

                if (book == null)
                    return ServiceResult<OrderResponse>.Failure($"Book with ID {item.BookId} was not found"); //i want to return book title instead of id

                if (item.Quantity <= 0)
                    return ServiceResult<OrderResponse>.Failure($"Quantity must be greater than zero");

                if (book.StockQuantity < item.Quantity)
                    return ServiceResult<OrderResponse>.Failure($"Not enough stock for book '{book.Title}'");

                var itemTotal = book.Price * item.Quantity;

                book.StockQuantity -= item.Quantity;

                orderItems.Add(new OrderItem  //we are creating order items based on the request and the book details
                {
                    BookId = book.Id,
                    BookTitle = book.Title,
                    Author = book.Author,
                    UnitPrice = book.Price,
                    Quantity = item.Quantity,
                    TotalPrice = itemTotal

                });
                totalAmount += itemTotal;
            }

            var order = new Order  //we are creating the order based on the user id, total amount and the order items we created above
            {
                UserId = userId.Value, //value means we are sure that userId is not null because we checked it above
                TotalAmount = totalAmount,
                OrderItems = orderItems
            };

            _context.Orders.Add(order); //we are adding the order to the database context
            await _context.SaveChangesAsync();


            var response = new OrderResponse  //we are creating the response based on the order we just created and saved to the database
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                CreatedAt = DateTime.UtcNow,
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

            return ServiceResult<OrderResponse>.Success(response, "Order created successfully."); 

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
