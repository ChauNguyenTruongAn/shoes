using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Addresses
{
    [Route("api/addresses")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        private int GetUserIdFromToken()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("Không xác định được danh tính.");
            return int.Parse(userIdString);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAddresses()
        {
            var userId = GetUserIdFromToken();
            var addresses = await _addressService.GetMyAddressesAsync(userId);
            return Ok(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAddressDto request)
        {
            var userId = GetUserIdFromToken();
            var result = await _addressService.CreateAsync(userId, request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAddressDto request)
        {
            var userId = GetUserIdFromToken();
            var success = await _addressService.UpdateAsync(userId, id, request);
            if (!success) return NotFound(new { message = "Không tìm thấy địa chỉ" });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var success = await _addressService.DeleteAsync(userId, id);
                if (!success) return NotFound(new { message = "Không tìm thấy địa chỉ" });
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/set-default")]
        public async Task<IActionResult> SetDefault(int id)
        {
            var userId = GetUserIdFromToken();
            var success = await _addressService.SetDefaultAsync(userId, id);
            if (!success) return NotFound(new { message = "Không tìm thấy địa chỉ" });
            return NoContent();
        }
    }
}