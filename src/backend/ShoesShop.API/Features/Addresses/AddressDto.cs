using System.ComponentModel.DataAnnotations;

namespace ShoesShop.API.Features.Addresses
{
    public class AddressResponseDto
    {
        public int Id { get; set; }
        public string ReceiverName { get; set; }
        public string Phone { get; set; }
        public string AddressLine { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CreateAddressDto
    {
        [Required] public string ReceiverName { get; set; }
        [Required] public string Phone { get; set; }
        [Required] public string AddressLine { get; set; }
        [Required] public string Ward { get; set; }
        [Required] public string District { get; set; }
        [Required] public string City { get; set; }
        public bool IsDefault { get; set; } = false;
    }

    public class UpdateAddressDto : CreateAddressDto 
    { 
        // Kế thừa lại toàn bộ thuộc tính của Create để code ngắn gọn
    }
}