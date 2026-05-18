using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesShop.API.Infrastructure.Database.Entities
{
    [Table("Users")]
    public class User : BaseEntity
    {
        [Required, MaxLength(255)]
        public string Email { get; set; } // Phải Unique, not null [cite: 1]

        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(20)]
        public string Role { get; set; } = "customer"; // Mặc định là 'customer' [cite: 1]

        [MaxLength(20)]
        public string Status { get; set; } = "active"; // Mặc định là 'active' [cite: 1]

        // Navigation
        public ICollection<Address> Addresses { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<UserPromotion> UserPromotions { get; set; }
    }

    [Table("Addresses")]
    public class Address : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        [MaxLength(100)]
        public string ReceiverName { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(255)]
        public string AddressLine { get; set; } // Số nhà, tên đường [cite: 1]

        [MaxLength(100)]
        public string Ward { get; set; }

        [MaxLength(100)]
        public string District { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        public bool IsDefault { get; set; } = false; // Mặc định false [cite: 1]
    }
}