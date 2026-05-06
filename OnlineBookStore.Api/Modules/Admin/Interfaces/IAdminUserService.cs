using OnlineBookStore.Api.Shared.Helpers;
using OnlineBookStore.Api.Modules.Admin.DTOs;

namespace OnlineBookStore.Api.Modules.Admin.Interfaces
{
    public interface IAdminUserService
    {
        Task<ServiceResult<List<UserResponse>>> GetAllUsersAsync();

        Task<ServiceResult<UserResponse>> UpdateUserRoleAsync( int userId, UpdateUserRoleRequest request );
    }
}
