using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Api.Modules.Auth.Interfaces;
using OnlineBookStore.Api.Modules.Auth.DTOs;


namespace OnlineBookStore.Api.Modules.Auth.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request); // Call the service to handle registration

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);


        }
    }
}