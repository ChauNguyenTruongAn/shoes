using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Categories
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        // Tiêm Dependency (Dependency Injection)
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound(new { message = "Không tìm thấy danh mục" });
            
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto request)
        {
            try
            {
                var result = await _categoryService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto request)
        {
            try
            {
                var success = await _categoryService.UpdateAsync(id, request);
                if (!success) return NotFound(new { message = "Không tìm thấy danh mục để cập nhật" });
                
                return NoContent(); // Code 204: Cập nhật thành công không trả về body
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy danh mục để xóa" });
            
            return NoContent();
        }
    }
}