using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Products
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto> GetByIdAsync(int id);
        Task<ProductResponseDto> CreateAsync(CreateProductDto request);
        Task<bool> UpdateAsync(int id, UpdateProductDto request);
        Task<bool> DeleteAsync(int id);
    }
}