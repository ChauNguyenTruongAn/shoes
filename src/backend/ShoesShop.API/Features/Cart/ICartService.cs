using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Cart
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemResponseDto>> GetCartAsync(int userId);
        Task<CartItemResponseDto> AddToCartAsync(int userId, AddToCartDto request);
        Task<bool> UpdateQuantityAsync(int userId, int cartItemId, UpdateCartItemDto request);
        Task<bool> RemoveFromCartAsync(int userId, int cartItemId);
        Task<bool> ClearCartAsync(int userId);
    }
}