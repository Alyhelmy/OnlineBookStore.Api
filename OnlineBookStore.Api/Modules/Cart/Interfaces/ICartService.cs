using OnlineBookStore.Api.Modules.Cart.DTOs;
using OnlineBookStore.Api.Shared.Helpers;

namespace OnlineBookStore.Api.Modules.Cart.Interfaces
{
    public interface ICartService
    {
        Task<ServiceResult<CartResponse>> GetCartAsync();
        Task<ServiceResult<CartResponse>> AddToCartAsync(AddToCartRequest request);
        Task<ServiceResult<CartResponse>> UpdateCartItemAsync(
            int cartItemId,
            UpdateCartItemRequest request);

        Task<ServiceResult<bool>> RemoveCartItemAsync(int cartItemId);
        Task<ServiceResult<bool>> ClearCartAsync();
    }
}
