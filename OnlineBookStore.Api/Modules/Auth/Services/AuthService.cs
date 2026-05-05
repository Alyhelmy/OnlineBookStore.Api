using Microsoft.EntityFrameworkCore;
using OnlineBookStore.Api.Modules.Auth.DTOs;
using OnlineBookStore.Api.Modules.Auth.Interfaces;
using OnlineBookStore.Api.Modules.Auth.Models;
using OnlineBookStore.Api.Shared.Data;
using OnlineBookStore.Api.Shared.Helpers;

namespace OnlineBookStore.Api.Modules.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }



        public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            var emailExists = await _context.Users
                .AnyAsync(user => user.Email == request.Email); //AnyAsync checks if any user exists with the given email

            if (emailExists)  
                return ServiceResult<AuthResponse>.Failure("Email already exists");

            var user = new User  // Create a new user entity based on the registration request
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                Role = "Customer"

            };

            _context.Users.Add(user);  
            await _context.SaveChangesAsync();

            return ServiceResult<AuthResponse>.Success(new AuthResponse
            {
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Token = _tokenService.CreateToken(user)
            }, "Registration successful");

        }

        public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.Email == request.Email);  //FirstOrDefaultAsync retrieves the first user that matches the email or returns null if no match is found

            if (user == null)  
                return ServiceResult<AuthResponse>.Failure("Incorrect Email or Password");


            var isPasswordValid = PasswordHasher.VerifyPassword(  // this checks if the provided password matches the stored password hash for the user
            request.Password,
            user.PasswordHash);  

            if(!isPasswordValid) 
                return ServiceResult<AuthResponse>.Failure("Incorrect Email or Password"); // we return the same error message for both cases to avoid giving away information about which part of the credentials is incorrect


            return ServiceResult<AuthResponse>.Success(new AuthResponse
            {
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Token = _tokenService.CreateToken(user)
            }, "Login successful");  

        }

    }
}
