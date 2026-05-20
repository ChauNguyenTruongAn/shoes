using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoesShop.API.Infrastructure.Database;
using ShoesShop.API.Infrastructure.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Products
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Variants) // Lấy luôn cả các biến thể

                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Variants)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto request)
        {
            // 1. Kiểm tra Slug sản phẩm
            if (await _context.Products.AnyAsync(p => p.Slug == request.Slug))
                throw new Exception("Slug sản phẩm đã tồn tại.");

            // 2. Kiểm tra danh sách SKU xem có bị trùng lặp không (SKU là mã vạch, phải Unique)
            if (request.Variants != null && request.Variants.Any())
            {
                var skus = request.Variants.Select(v => v.Sku).ToList();
                var duplicateSkus = await _context.ProductVariants
                    .Where(v => skus.Contains(v.Sku))
                    .Select(v => v.Sku)
                    .ToListAsync();

                if (duplicateSkus.Any())
                    throw new Exception($"Các SKU sau đã tồn tại trong hệ thống: {string.Join(", ", duplicateSkus)}");
            }

            // 3. Map từ DTO sang Entity. EF Core đủ thông minh để map luôn danh sách Variants đi kèm!
            var product = _mapper.Map<Product>(request);

            _context.Products.Add(product);
            await _context.SaveChangesAsync(); // Lưu 1 phát được cả cha lẫn con

            // Lấy lại dữ liệu đầy đủ để trả về UI (có kèm Tên Brand, Category)
            return await GetByIdAsync(product.Id);
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductDto request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            if (product.Slug != request.Slug && await _context.Products.AnyAsync(p => p.Slug == request.Slug))
                throw new Exception("Slug sản phẩm đã tồn tại.");

            _mapper.Map(request, product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (product == null) return false;

            // Xóa mềm Sản phẩm
            product.IsDeleted = true;
            
            // Phải xóa mềm luôn cả các biến thể của nó để tránh lỗi hiển thị giỏ hàng
            foreach (var variant in product.Variants)
            {
                variant.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}