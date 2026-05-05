using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OnlineBookStore.Api.Modules.Auth.Interfaces;
using OnlineBookStore.Api.Modules.Auth.Models;

namespace OnlineBookStore.Api.Modules.Auth.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration; // we inject the IConfiguration service into the TokenService class, which allows us to access configuration settings (like the JWT secret key) from our appsettings.json file or environment variables.
        }

        public string CreateToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];

            var claims = new List<Claim>  // we create a list of claims that will be included in the JWT token. These claims represent the user's identity and role, and they will be used by the application to authorize access to protected resources.
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));  // we create a symmetric security key using the JWT secret key from the configuration. This key will be used to sign the JWT token, ensuring its integrity and authenticity.

            var credintials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // we create signing credentials using the symmetric security key and specify the HMAC SHA256 algorithm for signing the token.

            var token = new JwtSecurityToken(  // we create a new JWT token using the JwtSecurityToken class. We specify the issuer, audience, claims, expiration time, and signing credentials for the token.
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims, 
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
                signingCredentials: credintials
                );

            return new JwtSecurityTokenHandler().WriteToken(token); // we use the JwtSecurityTokenHandler to write the token to a string format, which can be returned to the client and used for authentication in subsequent requests.

        }
    }
}