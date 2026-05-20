using Microsoft.EntityFrameworkCore;
using ShoesShop.API.Infrastructure.Database;
using System;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Promotions
{
    public interface IPromotionService
    {
        Task<PromotionResultDto> CalculateDiscountAsync(string code, decimal orderTotal);
    }

    public class PromotionService : IPromotionService
    {
        private readonly AppDbContext _context;

        public PromotionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PromotionResultDto> CalculateDiscountAsync(string code, decimal orderTotal)
        {
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code == code.ToUpper());

            // 1. Các bước Validate khắc nghiệt
            if (promotion == null || !promotion.IsActive)
                throw new Exception("Mã giảm giá không tồn tại hoặc đã bị khóa.");

            if (DateTime.UtcNow < promotion.StartDate || DateTime.UtcNow > promotion.EndDate)
                throw new Exception("Mã giảm giá đã hết hạn hoặc chưa đến thời gian sử dụng.");

            if (promotion.UsedCount >= promotion.UsageLimit)
                throw new Exception("Mã giảm giá đã hết lượt sử dụng.");

            if (orderTotal < promotion.MinOrderValue)
                throw new Exception($"Đơn hàng cần đạt tối thiểu {promotion.MinOrderValue:N0}đ để áp dụng mã này.");

            // 2. Tính toán số tiền được giảm
            decimal discountAmount = 0;

            switch (promotion.DiscountType.ToLower())
            {
                case "fixed_amount":
                    discountAmount = promotion.DiscountValue;
                    break;
                    
                case "percentage":
                    discountAmount = orderTotal * (promotion.DiscountValue / 100);
                    // Cực kỳ quan trọng: Áp dụng mức giảm tối đa (Ví dụ: Giảm 20% nhưng tối đa 100k)
                    if (discountAmount > promotion.MaxDiscountAmount && promotion.MaxDiscountAmount > 0)
                    {
                        discountAmount = promotion.MaxDiscountAmount;
                    }
                    break;
                    
                case "free_shipping":
                    // Sẽ được xử lý linh động bên phần Checkout, tạm thời trả về 0 để UI biết
                    discountAmount = 0; 
                    break;
            }

            // Đảm bảo không giảm lố tổng tiền đơn hàng
            if (discountAmount > orderTotal) discountAmount = orderTotal;

            return new PromotionResultDto
            {
                PromotionId = promotion.Id,
                Code = promotion.Code,
                DiscountType = promotion.DiscountType,
                DiscountAmount = discountAmount
            };
        }
    }
}