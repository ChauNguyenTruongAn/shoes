using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.API.Features.Payments;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Orders
{
    [Route("api/orders")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IVnPayService _vnPayService;

        public OrderController(IOrderService orderService, IVnPayService vnPayService)
        {
            _orderService = orderService;
            _vnPayService = vnPayService;
        }

        private string GetUserRoleFromToken()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "customer";
        }

        private int GetUserIdFromToken()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("Không xác định được danh tính.");
            return int.Parse(userIdString);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _orderService.CheckoutAsync(userId, request);

                // NẾU KHÁCH CHỌN VNPAY -> TRẢ VỀ URL ĐỂ REDIRECT
                if (request.PaymentMethod == "VNPAY")
                {
                    var vnPayModel = new PaymentInformationModel
                    {
                        OrderType = "other",
                        Amount = (double)result.TotalAmount,
                        OrderDescription = "Thanh toan giay",
                        Name = "Khach hang",
                        OrderId = result.Id
                    };
                    var paymentUrl = _vnPayService.CreatePaymentUrl(vnPayModel, HttpContext);
                    
                    return Ok(new { 
                        message = "Đơn hàng đã được tạo. Vui lòng thanh toán.", 
                        orderId = result.Id,
                        paymentUrl = paymentUrl // FE sẽ lấy link này để window.location.href
                    });
                }

                // Nếu COD thì trả về bình thường
                return Ok(new { message = "Đặt hàng thành công", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetMyOrderHistory()
        {
            var userId = GetUserIdFromToken();
            var orders = await _orderService.GetMyOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var userId = GetUserIdFromToken();
            var role = GetUserRoleFromToken();

            var order = await _orderService.GetOrderDetailsAsync(id, userId, role);
            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng hoặc bạn không có quyền xem đơn này" });

            return Ok(order);
        }

        // 3. API Dành riêng cho ADMIN cập nhật trạng thái vận chuyển và thanh toán
        [HttpPut("{id}/status")]
        [Authorize(Roles = "admin")] // Chỉ Token có quyền 'admin' mới gọi được Endpoint này
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto request)
        {
            var success = await _orderService.UpdateStatusAsync(id, request);
            if (!success) return NotFound(new { message = "Không tìm thấy đơn hàng để cập nhật trạng thái" });

            return NoContent();
        }
    }
}