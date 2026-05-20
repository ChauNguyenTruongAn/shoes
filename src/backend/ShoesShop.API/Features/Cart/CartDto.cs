namespace ShoesShop.API.Features.Cart
{
    public class CartItemResponseDto
    {
        public int Id { get; set; } // ID của dòng trong giỏ hàng
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; }
        public string ProductSlug { get; set; }
        public string Sku { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; } // Giá lúc này (có thể là giá gốc hoặc giá sale)
        public string VariantImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity; // Tự động tính tổng tiền của dòng này
    }

    public class AddToCartDto
    {
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemDto
    {
        public int Quantity { get; set; }
    }
}