using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Addresses
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressResponseDto>> GetMyAddressesAsync(int userId);
        Task<AddressResponseDto> GetByIdAsync(int userId, int addressId);
        Task<AddressResponseDto> CreateAsync(int userId, CreateAddressDto request);
        Task<bool> UpdateAsync(int userId, int addressId, UpdateAddressDto request);
        Task<bool> DeleteAsync(int userId, int addressId);
        Task<bool> SetDefaultAsync(int userId, int addressId);
    }
}