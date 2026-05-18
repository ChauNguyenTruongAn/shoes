using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Brands
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandResponseDto>> GetAllAsync();
        Task<BrandResponseDto> GetByIdAsync(int id);
        Task<BrandResponseDto> CreateAsync(CreateBrandDto request);
        Task<bool> UpdateAsync(int id, UpdateBrandDto request);
        Task<bool> DeleteAsync(int id);
    }
}