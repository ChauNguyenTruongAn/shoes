using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Promotions
{
    [Route("api/promotions")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyPromotion([FromBody] ApplyPromotionRequestDto request)
        {
            try
            {
                var result = await _promotionService.CalculateDiscountAsync(request.Code, request.OrderTotal);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}