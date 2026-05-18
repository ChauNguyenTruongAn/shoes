using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesShop.API.Infrastructure.Database.Entities
{
    [Table("Categories")]
    public class Category : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } // VD: Giày chạy bộ, Giày Sneaker [cite: 2]

        [Required, MaxLength(150)]
        public string Slug { get; set; } // Unique [cite: 2]

        public int? ParentId { get; set; } // Cho phép danh mục cha - con [cite: 2]
        public Category ParentCategory { get; set; }
        
        public ICollection<Category> SubCategories { get; set; }
        public ICollection<Product> Products { get; set; }
    }

    [Table("Brands")]
    public class Brand : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } // VD: Nike, Adidas, Puma [cite: 2]

        public string LogoUrl { get; set; }

        public ICollection<Product> Products { get; set; }
    }

    [Table("Product_Images")]
    public class ProductImage : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string ImageUrl { get; set; }

        public bool IsThumbnail { get; set; } = false; // Mặc định false [cite: 3]
    }
}