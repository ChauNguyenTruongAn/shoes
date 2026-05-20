using Microsoft.EntityFrameworkCore;
using ShoesShop.API.Infrastructure.Database;
using ShoesShop.API.Infrastructure.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Cart
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItemResponseDto>> GetCartAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Include(c => c.ProductVariant)
                .ThenInclude(v => v.Product) // Kéo luôn thông tin Product gốc ra
                .Where(c => c.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

            return cartItems.Select(c => new CartItemResponseDto
            {
                Id = c.Id,
                ProductVariantId = c.ProductVariantId,
                ProductName = c.ProductVariant.Product.Name,
                ProductSlug = c.ProductVariant.Product.Slug,
                Sku = c.ProductVariant.Sku,
                Color = c.ProductVariant.Color,
                Size = c.ProductVariant.Size,
                // Ưu tiên lấy giá Khuyến mãi nếu có, không thì lấy giá gốc
                Price = c.ProductVariant.PromotionalPrice ?? c.ProductVariant.Price,
                VariantImageUrl = c.ProductVariant.VariantImageUrl,
                Quantity = c.Quantity
            });
        }

        public async Task<CartItemResponseDto> AddToCartAsync(int userId, AddToCartDto request)
        {
            // 1. Kiểm tra biến thể có tồn tại và còn hàng không
            var variant = await _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == request.ProductVariantId);

            if (variant == null) throw new Exception("Sản phẩm không tồn tại.");
            if (variant.StockQuantity < request.Quantity) throw new Exception("Số lượng tồn kho không đủ.");

            // 2. Kiểm tra xem trong giỏ đã có món này chưa
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductVariantId == request.ProductVariantId);

            if (existingItem != null)
            {
                // Nếu có rồi thì cộng dồn
                if (existingItem.Quantity + request.Quantity > variant.StockQuantity)
                    throw new Exception("Tổng số lượng vượt quá tồn kho.");
                
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                // Nếu chưa có thì tạo mới
                var newItem = new CartItem
                {
                    UserId = userId,
                    ProductVariantId = request.ProductVariantId,
                    Quantity = request.Quantity
                };
                _context.CartItems.Add(newItem);
            }

            await _context.SaveChangesAsync();

            // Gọi lại hàm Get để trả về full data cho cái item vừa thêm (trick để đỡ phải map tay lại)
            var allItems = await GetCartAsync(userId);
            return allItems.FirstOrDefault(c => c.ProductVariantId == request.ProductVariantId);
        }

        public async Task<bool> UpdateQuantityAsync(int userId, int cartItemId, UpdateCartItemDto request)
        {
            var cartItem = await _context.CartItems
                .Include(c => c.ProductVariant)
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

            if (cartItem == null) return false;

            if (request.Quantity > cartItem.ProductVariant.StockQuantity)
                throw new Exception("Số lượng tồn kho không đủ.");

            if (request.Quantity <= 0)
            {
                // Nếu update số lượng về 0 hoặc âm thì xóa luôn khỏi giỏ
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = request.Quantity;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int userId, int cartItemId)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
            if (cartItem == null) return false;

            // Xóa cứng (Hard Delete) khỏi CSDL vì giỏ hàng không cần thiết phải xóa mềm lưu rác
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var items = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}