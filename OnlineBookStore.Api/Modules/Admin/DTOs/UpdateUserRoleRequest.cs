using System.ComponentModel.DataAnnotations;

namespace OnlineBookStore.Api.Modules.Admin.DTOs
{
    public class UpdateUserRoleRequest
    {
        [Required]
        public string Role {  get; set; } = string.Empty;
    }
}
