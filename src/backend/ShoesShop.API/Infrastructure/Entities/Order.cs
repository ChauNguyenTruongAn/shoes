using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesShop.API.Infrastructure.Database.Entities
{
    [Table("Orders")]
    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public string Notes { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } // Tổng tiền CẦN THANH TOÁN (Đã cộng ship, trừ voucher) [cite: 5]

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }

        public int? PromotionId { get; set; } // Cho phép null vì không phải đơn nào cũng có mã [cite: 6]
        public Promotion Promotion { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0; // Số tiền được giảm thực tế nhờ Voucher [cite: 6]

        [MaxLength(50)]
        public string Status { get; set; }

        [MaxLength(50)]
        public string PaymentMethod { get; set; }

        [MaxLength(50)]
        public string PaymentStatus { get; set; }

        public int ShippingAddressId { get; set; }
        public Address ShippingAddress { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }

    [Table("Order_Items")]
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductVariantId { get; set; } // Giữ lại ID biến thể khách đã mua [cite: 6]
        public ProductVariant ProductVariant { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } // Lưu lại giá tại thời điểm mua (tránh việc đổi giá sau này làm sai lịch sử) [cite: 7]
    }
}