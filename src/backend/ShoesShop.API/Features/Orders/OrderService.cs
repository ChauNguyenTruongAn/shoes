using Microsoft.EntityFrameworkCore;
using ShoesShop.API.Infrastructure.Database;
using ShoesShop.API.Infrastructure.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Orders
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderResponseDto> CheckoutAsync(int userId, CheckoutRequestDto request)
        {
            // 1. Khởi tạo Transaction để đảm bảo tính toàn vẹn dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 2. Lấy dữ liệu Giỏ hàng và Địa chỉ
                var cartItems = await _context.CartItems
                    .Include(c => c.ProductVariant)
                    .ThenInclude(v => v.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                    throw new Exception("Giỏ hàng của bạn đang trống.");

                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == request.ShippingAddressId && a.UserId == userId);
                
                if (address == null)
                    throw new Exception("Địa chỉ giao hàng không hợp lệ.");

                // 3. Khởi tạo Đơn hàng (Order)
                var order = new Order
                {
                    UserId = userId,
                    ShippingAddressId = address.Id,
                    PaymentMethod = request.PaymentMethod,
                    PaymentStatus = "unpaid", // Mặc định là chưa thanh toán
                    Status = "pending",       // Chờ shop xác nhận
                    Notes = request.Notes,
                    ShippingFee = 30000,      // Fix cứng phí ship tạm thời (có thể làm logic tính theo tỉnh thành sau)
                    DiscountAmount = 0        // Sẽ xử lý ở phần Khuyến mãi
                };

                decimal subTotal = 0;
                var orderItems = new List<OrderItem>();

                // 4. Xử lý từng dòng sản phẩm: Tính tiền, Trừ tồn kho và Tạo OrderItem
                foreach (var cartItem in cartItems)
                {
                    var variant = cartItem.ProductVariant;

                    // KIỂM TRA TỒN KHO LẦN CUỐI CỰC KỲ QUAN TRỌNG
                    if (variant.StockQuantity < cartItem.Quantity)
                        throw new Exception($"Sản phẩm '{variant.Product.Name} - {variant.Color} - Size {variant.Size}' chỉ còn {variant.StockQuantity} sản phẩm trong kho.");

                    // Lấy giá ưu đãi nếu có
                    decimal currentPrice = variant.PromotionalPrice ?? variant.Price;
                    
                    subTotal += currentPrice * cartItem.Quantity;

                    // TRỪ TỒN KHO
                    variant.StockQuantity -= cartItem.Quantity;

                    // Thêm vào danh sách chi tiết hóa đơn
                    orderItems.Add(new OrderItem
                    {
                        ProductVariantId = variant.Id,
                        Quantity = cartItem.Quantity,
                        UnitPrice = currentPrice // Lưu cứng giá lúc mua
                    });
                }

                // 4.5. XỬ LÝ KHUYẾN MÃI (VOUCHER)
                if (!string.IsNullOrEmpty(request.PromotionCode))
                {
                    var promotion = await _context.Promotions
                        .FirstOrDefaultAsync(p => p.Code == request.PromotionCode.ToUpper());

                    if (promotion == null || !promotion.IsActive || 
                        DateTime.UtcNow < promotion.StartDate || DateTime.UtcNow > promotion.EndDate ||
                        promotion.UsedCount >= promotion.UsageLimit || subTotal < promotion.MinOrderValue)
                    {
                        throw new Exception("Mã giảm giá không hợp lệ hoặc không đủ điều kiện áp dụng.");
                    }

                    // Tính tiền giảm
                    if (promotion.DiscountType == "percentage")
                    {
                        order.DiscountAmount = subTotal * (promotion.DiscountValue / 100);
                        if (order.DiscountAmount > promotion.MaxDiscountAmount && promotion.MaxDiscountAmount > 0)
                            order.DiscountAmount = promotion.MaxDiscountAmount;
                    }
                    else if (promotion.DiscountType == "fixed_amount")
                    {
                        order.DiscountAmount = promotion.DiscountValue;
                    }
                    else if (promotion.DiscountType == "free_shipping")
                    {
                        // Miễn phí vận chuyển thì gán luôn tiền ship bằng 0
                        order.ShippingFee = 0; 
                    }

                    // Lưu ID promotion vào hóa đơn để đối soát và tăng số lượt sử dụng
                    order.PromotionId = promotion.Id;
                    promotion.UsedCount += 1; 
                }

                // Đảm bảo giảm giá không vượt quá tiền hàng
                if (order.DiscountAmount > subTotal) order.DiscountAmount = subTotal;

                // Chốt tổng tiền cần thanh toán
                order.TotalAmount = subTotal + order.ShippingFee - order.DiscountAmount;
                order.OrderItems = orderItems;

                _context.Orders.Add(order);

                

                // 5. Xóa giỏ hàng sau khi đặt hàng thành công
                _context.CartItems.RemoveRange(cartItems);

                // 6. Lưu xuống DB và Xác nhận Transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); // Mọi thứ suôn sẻ thì chốt sổ!

                // 7. Trả về kết quả
                return new OrderResponseDto
                {
                    Id = order.Id,
                    TotalAmount = order.TotalAmount,
                    ShippingFee = order.ShippingFee,
                    DiscountAmount = order.DiscountAmount,
                    Status = order.Status,
                    PaymentMethod = order.PaymentMethod,
                    PaymentStatus = order.PaymentStatus,
                    Notes = order.Notes,
                    Items = orderItems.Select(oi => new OrderItemResponseDto
                    {
                        ProductVariantId = oi.ProductVariantId,
                        ProductName = cartItems.First(c => c.ProductVariantId == oi.ProductVariantId).ProductVariant.Product.Name,
                        Color = cartItems.First(c => c.ProductVariantId == oi.ProductVariantId).ProductVariant.Color,
                        Size = cartItems.First(c => c.ProductVariantId == oi.ProductVariantId).ProductVariant.Size,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList()
                };
            }
            catch (Exception)
            {
                // Nếu có bất kỳ lỗi nào (hết hàng, đứt mạng giữa chừng), Rollback lại toàn bộ!
                await transaction.RollbackAsync();
                throw; 
            }
        }

        public async Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt) // Đơn mới nhất xếp lên đầu
                .AsNoTracking()
                .ToListAsync();

            return orders.Select(MapToOrderResponseDto);
        }

        // 2. Xem chi tiết 1 đơn hàng (Bảo mật: Khách hàng chỉ xem được đơn của mình, Admin xem được hết)
        public async Task<OrderResponseDto> GetOrderDetailsAsync(int orderId, int userId, string userRole)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(pv => pv.Product).AsNoTracking(); // Tối ưu câu lệnh SQL

            Order order;
            if (userRole == "admin")
            {
                order = await query.FirstOrDefaultAsync(o => o.Id == orderId);
            }
            else
            {
                order = await query.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            }

            if (order == null) return null;

            return MapToOrderResponseDto(order);
        }

        // 3. Admin cập nhật trạng thái đơn hàng & Trạng thái thanh toán
        public async Task<bool> UpdateStatusAsync(int orderId, UpdateOrderStatusDto request)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant) // Kéo variant ra phòng trường hợp hoàn kho
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return false;

            // Nghiệp vụ nâng cao: Nếu đơn hàng bị HỦY (cancelled), phải HOÀN LẠI số lượng tồn kho cho shop
            if (request.Status.ToLower() == "cancelled" && order.Status.ToLower() != "cancelled")
            {
                foreach (var item in order.OrderItems)
                {
                    item.ProductVariant.StockQuantity += item.Quantity;
                }
            }

            order.Status = request.Status;
            order.PaymentStatus = request.PaymentStatus;

            await _context.SaveChangesAsync();
            return true;
        }

        // Helper Method để tự động ánh xạ dữ liệu (tránh lặp code chuyển đổi object)
        private OrderResponseDto MapToOrderResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                ShippingFee = order.ShippingFee,
                DiscountAmount = order.DiscountAmount,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                Notes = order.Notes,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ProductVariantId = oi.ProductVariantId,
                    ProductName = oi.ProductVariant?.Product?.Name ?? "Sản phẩm không tồn tại",
                    Color = oi.ProductVariant?.Color,
                    Size = oi.ProductVariant?.Size,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
    }
}