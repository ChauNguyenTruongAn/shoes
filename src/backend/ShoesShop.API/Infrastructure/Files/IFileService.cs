using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ShoesShop.API.Infrastructure.Files
{
    public interface IFileService
    {
        // Nhận vào 1 file, trả về đường dẫn tương đối (VD: /uploads/products/abc.jpg)
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        
        // Xóa file nếu người dùng xóa sản phẩm (dọn dẹp ổ cứng)
        void DeleteFile(string fileUrl);
    }
}