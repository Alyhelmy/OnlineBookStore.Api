using System.ComponentModel.DataAnnotations;

namespace OnlineBookStore.Api.Modules.Auth.DTOs
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(50,MinimumLength =2)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [RegularExpression(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Please enter a valid email address."
        )]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }
}
