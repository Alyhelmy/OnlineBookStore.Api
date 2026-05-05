using System.ComponentModel.DataAnnotations;

namespace OnlineBookStore.Api.Modules.Orders.DTOs;

public class UpdateOrderStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
}