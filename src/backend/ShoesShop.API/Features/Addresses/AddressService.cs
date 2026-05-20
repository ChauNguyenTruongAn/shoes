using Microsoft.EntityFrameworkCore;
using ShoesShop.API.Infrastructure.Database;
using ShoesShop.API.Infrastructure.Database.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Addresses
{
    public class AddressService : IAddressService
    {
        private readonly AppDbContext _context;

        public AddressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AddressResponseDto>> GetMyAddressesAsync(int userId)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault) // Địa chỉ mặc định luôn nổi lên đầu
                .AsNoTracking()
                .ToListAsync();

            return addresses.Select(MapToDto);
        }

        public async Task<AddressResponseDto> GetByIdAsync(int userId, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
            
            if (address == null) return null;
            return MapToDto(address);
        }

        public async Task<AddressResponseDto> CreateAsync(int userId, CreateAddressDto request)
        {
            var hasExistingAddress = await _context.Addresses.AnyAsync(a => a.UserId == userId);
            
            // Nếu đây là địa chỉ ĐẦU TIÊN của khách, tự động ép nó thành Mặc định
            var isDefault = !hasExistingAddress || request.IsDefault;

            if (isDefault && hasExistingAddress)
            {
                await RemoveOldDefault(userId);
            }

            var address = new Address
            {
                UserId = userId,
                ReceiverName = request.ReceiverName,
                Phone = request.Phone,
                AddressLine = request.AddressLine,
                Ward = request.Ward,
                District = request.District,
                City = request.City,
                IsDefault = isDefault
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return MapToDto(address);
        }

        public async Task<bool> UpdateAsync(int userId, int addressId, UpdateAddressDto request)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
            
            if (address == null) return false;

            if (request.IsDefault && !address.IsDefault)
            {
                await RemoveOldDefault(userId);
            }
            // Không cho phép user tự tắt IsDefault nếu nó đang là true. (Phải chọn cái khác làm default thay thế).
            else if (!request.IsDefault && address.IsDefault)
            {
                request.IsDefault = true; 
            }

            address.ReceiverName = request.ReceiverName;
            address.Phone = request.Phone;
            address.AddressLine = request.AddressLine;
            address.Ward = request.Ward;
            address.District = request.District;
            address.City = request.City;
            address.IsDefault = request.IsDefault;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int userId, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
                
            if (address == null) return false;

            if (address.IsDefault)
                throw new System.Exception("Không thể xóa địa chỉ mặc định. Vui lòng chọn địa chỉ khác làm mặc định trước.");

            _context.Addresses.Remove(address); // Địa chỉ có thể xóa cứng
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefaultAsync(int userId, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
                
            if (address == null) return false;

            await RemoveOldDefault(userId);
            address.IsDefault = true;
            
            await _context.SaveChangesAsync();
            return true;
        }

        // ============ HÀM TIỆN ÍCH (PRIVATE) ============
        private async Task RemoveOldDefault(int userId)
        {
            var oldDefault = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
            if (oldDefault != null)
            {
                oldDefault.IsDefault = false;
            }
        }

        private AddressResponseDto MapToDto(Address a)
        {
            return new AddressResponseDto
            {
                Id = a.Id,
                ReceiverName = a.ReceiverName,
                Phone = a.Phone,
                AddressLine = a.AddressLine,
                Ward = a.Ward,
                District = a.District,
                City = a.City,
                IsDefault = a.IsDefault
            };
        }
    }
}