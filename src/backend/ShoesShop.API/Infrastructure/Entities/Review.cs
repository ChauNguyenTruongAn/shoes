using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesShop.API.Infrastructure.Database.Entities
{
    [Table("Reviews")]
    public class Review : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; } // Đánh giá theo sản phẩm chung [cite: 7]
        public Product Product { get; set; }

        public int Rating { get; set; } // 1 tới 5 [cite: 7]

        public string Comment { get; set; }
    }

    [Table("User_Promotions")]
    public class UserPromotion : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int PromotionId { get; set; }
        public Promotion Promotion { get; set; }

        public bool IsUsed { get; set; } = false; // Đánh dấu user này đã xài mã này chưa (tránh 1 user xài 1 mã nhiều lần) [cite: 10]
    }
}