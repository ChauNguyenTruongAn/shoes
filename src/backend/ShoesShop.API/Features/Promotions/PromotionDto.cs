using System.ComponentModel.DataAnnotations;

namespace ShoesShop.API.Features.Promotions
{
    public class ApplyPromotionRequestDto
    {
        [Required]
        public string Code { get; set; }
        
        [Required]
        public decimal OrderTotal { get; set; } // Tổng tiền tạm tính để check điều kiện
    }

    public class PromotionResultDto
    {
        public int PromotionId { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountAmount { get; set; } // Số tiền ĐƯỢC GIẢM thực tế
    }
}