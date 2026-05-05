using OnlineBookStore.Api.Modules.Orders.DTOs;
using OnlineBookStore.Api.Shared.Helpers;

namespace OnlineBookStore.Api.Modules.Orders.Interfaces
{
    public interface IOrderService
    {

        Task<ServiceResult<OrderResponse>> CreateOrderAsync(CreateOrderRequest request); // Create a new order for the currently authenticated user

        Task<ServiceResult<List<OrderResponse>>> GetMyOrdersAsync(); // Get orders for the currently authenticated user

        Task<ServiceResult<OrderResponse>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request); // Update the status of an order (admin only)

        Task<ServiceResult<List<OrderResponse>>> GetAllOrdersAsync();

    }
}
