using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Cart
{
    [Route("api/cart")]
    [ApiController]
    [Authorize] // BẮT BUỘC ĐĂNG NHẬP (CÓ TOKEN MỚI ĐƯỢC VÀO)
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Hàm hỗ trợ lấy UserId từ Token
        private int GetUserIdFromToken()
        {
            // Trong AuthService chúng ta đã lưu Id vào JwtRegisteredClaimNames.Sub
            // ASP.NET tự map Sub thành ClaimTypes.NameIdentifier
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("Không xác định được danh tính.");
            
            return int.Parse(userIdString);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var userId = GetUserIdFromToken();
            var cartItems = await _cartService.GetCartAsync(userId);
            return Ok(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _cartService.AddToCartAsync(userId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromBody] UpdateCartItemDto request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var success = await _cartService.UpdateQuantityAsync(userId, cartItemId, request);
                if (!success) return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ" });
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = GetUserIdFromToken();
            var success = await _cartService.RemoveFromCartAsync(userId, cartItemId);
            if (!success) return NotFound(new { message = "Không tìm thấy sản phẩm trong giỏ" });
            
            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserIdFromToken();
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}