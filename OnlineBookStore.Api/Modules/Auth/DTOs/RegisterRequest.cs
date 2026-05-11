using System.ComponentModel.DataAnnotations;

namespace OnlineBookStore.Api.Modules.Auth.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Full name must be between 6 and 100 characters.")]
        [RegularExpression(
        @"^[\p{L}\p{M}'\-\s]+$",
        ErrorMessage = "Full name can only contain letters, spaces, hyphens, and apostrophes."
    )]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
        [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character."
    )]
        public string Password { get; set; } = string.Empty;
    }
}
