using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoesShop.API.Infrastructure.Database.Entities
{
    [Table("Products")]
    public class Product : BaseEntity
    {
        public int CategoryId { get; set; }
        public int BrandId { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string Slug { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        public Category Category { get; set; }
        public Brand Brand { get; set; }

        // Navigation properties
        public ICollection<ProductVariant> Variants { get; set; }
    }
}