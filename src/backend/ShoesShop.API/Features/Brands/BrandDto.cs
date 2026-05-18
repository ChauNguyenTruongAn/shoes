namespace ShoesShop.API.Features.Brands
{
    public class BrandResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
    }

    public class CreateBrandDto
    {
        public string Name { get; set; }
        public string LogoUrl { get; set; }
    }

    public class UpdateBrandDto
    {
        public string Name { get; set; }
        public string LogoUrl { get; set; }
    }
}