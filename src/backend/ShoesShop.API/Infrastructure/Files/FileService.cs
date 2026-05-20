using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ShoesShop.API.Infrastructure.Files
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        // IWebHostEnvironment giúp lấy đường dẫn vật lý tới thư mục wwwroot của máy chủ
        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ");

            // 1. Tạo đường dẫn tới thư mục lưu (VD: wwwroot/uploads/products)
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", folderName);

            // Nếu thư mục chưa tồn tại thì tạo mới
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // 2. Tạo tên file độc nhất để tránh bị trùng đè file của nhau
            // Đổi ảnh "giay.jpg" thành "12345678-giay.jpg"
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            // 3. Copy luồng dữ liệu của file vào ổ cứng server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 4. Trả về đường dẫn tương đối để lưu vào Database
            return $"/uploads/{folderName}/{fileName}";
        }

        public void DeleteFile(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            // Xóa tiền tố "/uploads/" để lấy đường dẫn thực tế
            var relativePath = fileUrl.TrimStart('/');
            var absolutePath = Path.Combine(_env.WebRootPath, relativePath);

            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }
        }
    }
}