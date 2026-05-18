using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesShop.API.Infrastructure.Database.Entities
{
    [Table("Product_Variants")]
    public class ProductVariant : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required, MaxLength(100)]
        public string Sku { get; set; }

        [MaxLength(50)]
        public string Color { get; set; }

        [MaxLength(20)]
        public string Size { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PromotionalPrice { get; set; }

        public int StockQuantity { get; set; }

        public string VariantImageUrl { get; set; }
    }
}