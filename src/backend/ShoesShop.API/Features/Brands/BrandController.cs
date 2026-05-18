using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Brands
{
    [Route("api/brands")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _brandService.GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand == null) return NotFound(new { message = "Không tìm thấy thương hiệu" });
            
            return Ok(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBrandDto request)
        {
            var result = await _brandService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandDto request)
        {
            var success = await _brandService.UpdateAsync(id, request);
            if (!success) return NotFound(new { message = "Không tìm thấy thương hiệu để cập nhật" });
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _brandService.DeleteAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy thương hiệu để xóa" });
            
            return NoContent();
        }
    }
}