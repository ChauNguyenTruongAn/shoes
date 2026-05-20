using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoesShop.API.Infrastructure.Files;
using System;
using System.Threading.Tasks;

namespace ShoesShop.API.Features.Uploads
{
    [Route("api/uploads")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IFileService _fileService;

        public UploadController(IFileService fileService)
        {
            _fileService = fileService;
        }

        // Endpoint tải lên 1 ảnh
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder = "products")
        {
            try
            {
                // Kiểm tra đuôi file (Chỉ cho phép ảnh)
                var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (extension != ".jpg" && extension != ".png" && extension != ".jpeg" && extension != ".webp")
                {
                    return BadRequest(new { message = "Chỉ hỗ trợ định dạng JPG, PNG, WEBP" });
                }

                // Giới hạn dung lượng (VD: 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "Dung lượng ảnh tối đa là 5MB" });
                }

                var fileUrl = await _fileService.UploadFileAsync(file, folder);
                
                // Trả về URL để Frontend nhận được và gán vào DTO khi tạo Product
                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}