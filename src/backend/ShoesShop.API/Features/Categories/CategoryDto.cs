namespace ShoesShop.API.Features.Categories
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? ParentId { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? ParentId { get; set; }
    }

    public class UpdateCategoryDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? ParentId { get; set; }
    }
}