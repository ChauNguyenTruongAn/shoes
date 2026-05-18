using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoesShop.API.Infrastructure.Database;
using ShoesShop.API.Infrastructure.Database.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Brands
{
    public class BrandService : IBrandService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper; // Tiêm AutoMapper vào

        public BrandService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BrandResponseDto>> GetAllAsync()
        {
            var brands = await _context.Brands.AsNoTracking().ToListAsync();
            // Dùng AutoMapper để chuyển 1 List<Brand> thành List<BrandResponseDto> chỉ với 1 dòng
            return _mapper.Map<IEnumerable<BrandResponseDto>>(brands);
        }

        public async Task<BrandResponseDto> GetByIdAsync(int id)
        {
            var brand = await _context.Brands.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
            if (brand == null) return null;

            return _mapper.Map<BrandResponseDto>(brand);
        }

        public async Task<BrandResponseDto> CreateAsync(CreateBrandDto request)
        {
            // Chuyển từ DTO sang Entity
            var brand = _mapper.Map<Brand>(request);

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return _mapper.Map<BrandResponseDto>(brand);
        }

        public async Task<bool> UpdateAsync(int id, UpdateBrandDto request)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return false;

            // Ghi đè dữ liệu từ DTO lên Entity có sẵn
            _mapper.Map(request, brand);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return false;

            brand.IsDeleted = true; // Xóa mềm
            await _context.SaveChangesAsync();
            return true;
        }
    }
}