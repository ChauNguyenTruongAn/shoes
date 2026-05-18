using Microsoft.EntityFrameworkCore;
using ShoesShop.API.Infrastructure.Database;
using ShoesShop.API.Infrastructure.Database.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            // Dùng AsNoTracking() để tăng hiệu năng khi chỉ đọc dữ liệu
            var categories = await _context.Categories.AsNoTracking().ToListAsync();

            return categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                ParentId = c.ParentId
            });
        }

        public async Task<CategoryResponseDto> GetByIdAsync(int id)
        {
            var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            
            if (category == null) return null;

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                ParentId = category.ParentId
            };
        }

        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto request)
        {
            // Kiểm tra trùng Slug (rất quan trọng cho SEO)
            if (await _context.Categories.AnyAsync(c => c.Slug == request.Slug))
            {
                throw new System.Exception("Slug đã tồn tại. Vui lòng chọn Slug khác.");
            }

            var category = new Category
            {
                Name = request.Name,
                Slug = request.Slug,
                ParentId = request.ParentId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                ParentId = category.ParentId
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto request)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            // Kiểm tra trùng Slug nếu có đổi Slug
            if (category.Slug != request.Slug && await _context.Categories.AnyAsync(c => c.Slug == request.Slug))
            {
                throw new System.Exception("Slug đã tồn tại. Vui lòng chọn Slug khác.");
            }

            category.Name = request.Name;
            category.Slug = request.Slug;
            category.ParentId = request.ParentId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            // Xóa mềm (Soft Delete) - Không gọi _context.Categories.Remove()
            category.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}