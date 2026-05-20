using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Payments
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        // URL này VNPay sẽ tự động gọi (redirect) về sau khi khách thanh toán xong trên web VNPay
        [HttpGet("vnpay-return")]
        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.Success)
            {
                // TODO: Ở đây bạn gọi _orderService.UpdatePaymentStatus(orderId, "paid") để update CSDL
                return Ok(new { message = "Thanh toán thành công!", data = response });
            }

            return BadRequest(new { message = "Thanh toán thất bại hoặc bị hủy.", data = response });
        }
    }
}