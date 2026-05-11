using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStore.Api.Modules.Cart.DTOs;
using OnlineBookStore.Api.Modules.Cart.Interfaces;

namespace OnlineBookStore.Api.Modules.Cart.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var result = await _cartService.GetCartAsync();
            return Ok(result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart(AddToCartRequest request)
        {
            {
                var result = await _cartService.AddToCartAsync(request);

                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
        }

        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateCartItem(
        int id,
        UpdateCartItemRequest request)
        {
            var result = await _cartService.UpdateCartItemAsync(id, request);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> RemoveCartItem(int id)
        {
            var result = await _cartService.RemoveCartItemAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _cartService.ClearCartAsync();

            return Ok(result);
        }

    }
}