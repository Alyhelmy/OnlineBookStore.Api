using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Api.Modules.Admin.DTOs;
using OnlineBookStore.Api.Modules.Admin.Interfaces;
using OnlineBookStore.Api.Modules.Admin.Services;

namespace OnlineBookStore.Api.Modules.Admin.Controllers
{

    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _adminUserService.GetAllUsersAsync();

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);

        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(
            int id,
            UpdateUserRoleRequest request)
        {
            var result = await _adminUserService.UpdateUserRoleAsync(id, request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
