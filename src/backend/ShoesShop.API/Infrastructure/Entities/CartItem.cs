using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesShop.API.Infrastructure.Database.Entities
{
    [Table("Cart_Items")]
    public class CartItem : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ProductVariantId { get; set; } // Liên kết tới biến thể cụ thể [cite: 5]
        public ProductVariant ProductVariant { get; set; }

        public int Quantity { get; set; }
    }
}