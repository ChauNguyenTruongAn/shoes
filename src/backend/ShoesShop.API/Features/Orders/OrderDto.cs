using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoesShop.API.Features.Orders
{
    public class CheckoutRequestDto
    {
        [Required]
        public int ShippingAddressId { get; set; }
        
        [Required]
        public string PaymentMethod { get; set; } // VD: "COD" hoặc "VNPAY"
        
        public string Notes { get; set; } // Ghi chú của khách hàng (giao giờ hành chính,...)
        
        // Khuyến mãi sẽ được xử lý ở bài tiếp theo, tạm thời để sẵn field
        public string PromotionCode { get; set; } 
    }

    public class OrderItemResponseDto
    {
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string Notes { get; set; }
        public List<OrderItemResponseDto> Items { get; set; }
    }
    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } // 'pending', 'processing', 'shipping', 'delivered', 'cancelled'
        
        [Required]
        public string PaymentStatus { get; set; } // 'unpaid', 'paid', 'refunded'
    }
}