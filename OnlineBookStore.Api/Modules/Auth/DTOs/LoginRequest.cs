using System.ComponentModel.DataAnnotations;

namespace OnlineBookStore.Api.Modules.Auth.DTOs
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [RegularExpression(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter a valid email address."
        )]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
