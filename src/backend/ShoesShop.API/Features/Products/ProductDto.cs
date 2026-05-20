using System.Collections.Generic;

namespace ShoesShop.API.Features.Products
{
    // ================= DTO CHO VARIANT =================
    public class ProductVariantResponseDto
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionalPrice { get; set; }
        public int StockQuantity { get; set; }
        public string VariantImageUrl { get; set; }
    }

    public class CreateProductVariantDto
    {
        public string Sku { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string VariantImageUrl { get; set; }
    }

    // ================= DTO CHO PRODUCT =================
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } // Phẳng hóa dữ liệu để UI dễ hiển thị
        public int BrandId { get; set; }
        public string BrandName { get; set; }    // Phẳng hóa dữ liệu
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        
        // Danh sách biến thể đi kèm
        public List<ProductVariantResponseDto> Variants { get; set; }
    }

    public class CreateProductDto
    {
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }

        // Cho phép tạo luôn danh sách biến thể khi tạo sản phẩm mới
        public List<CreateProductVariantDto> Variants { get; set; }
    }

    public class UpdateProductDto
    {
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        // Việc update Variant thường được tách ra thành 1 API riêng rẽ cho an toàn
    }
}