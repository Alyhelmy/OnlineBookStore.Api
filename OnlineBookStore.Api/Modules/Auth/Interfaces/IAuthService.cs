using OnlineBookStore.Api.Modules.Auth.DTOs;
using OnlineBookStore.Api.Shared.Helpers;

namespace OnlineBookStore.Api.Modules.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request); // serviceResult for user registration, accepting a RegisterRequest and returning a ServiceResult that may contain an AuthResponse if the registration is successful.
        Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request); 
    }
}
