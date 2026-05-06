using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Api.Modules.Admin.DTOs;
using OnlineBookStore.Api.Modules.Admin.Interfaces;
using OnlineBookStore.Api.Shared.Data;
using OnlineBookStore.Api.Shared.Helpers;
using System.ComponentModel;

namespace OnlineBookStore.Api.Modules.Admin.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly AppDbContext _context;

        public AdminUserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<List<UserResponse>>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .OrderByDescending(user => user.CreatedAt)
                .Select(user => new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt

                })
                .ToListAsync();

            return ServiceResult<List<UserResponse>>.Success(users, "Users Retrieved Successfully.");
        }

        public async Task<ServiceResult<UserResponse>> UpdateUserRoleAsync(
            int userId,
            UpdateUserRoleRequest request)
        {
            var allowedRoles = new[] { "Customer", "Admin" };

            if (!allowedRoles.Contains(request.Role))
                return ServiceResult<UserResponse>.Failure("invalid role.");

            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.Id == userId);

            if (user == null)
                return ServiceResult<UserResponse>.Failure("User with ID {userId was not found.");

            user.Role = request.Role;
            await _context.SaveChangesAsync();

            var response = new UserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return ServiceResult<UserResponse>.Success(response, "User Role updated Successfully");


        }
    }
}
