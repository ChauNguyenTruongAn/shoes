using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto> GetByIdAsync(int id);
        Task<CategoryResponseDto> CreateAsync(CreateCategoryDto request);
        Task<bool> UpdateAsync(int id, UpdateCategoryDto request);
        Task<bool> DeleteAsync(int id);
    }
}