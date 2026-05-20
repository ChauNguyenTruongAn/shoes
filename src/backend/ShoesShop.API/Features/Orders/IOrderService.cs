using System.Threading.Tasks;

namespace ShoesShop.API.Features.Orders
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CheckoutAsync(int userId, CheckoutRequestDto request);

        Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(int userId);
        Task<OrderResponseDto> GetOrderDetailsAsync(int orderId, int userId, string userRole);
        Task<bool> UpdateStatusAsync(int orderId, UpdateOrderStatusDto request);
    }
}