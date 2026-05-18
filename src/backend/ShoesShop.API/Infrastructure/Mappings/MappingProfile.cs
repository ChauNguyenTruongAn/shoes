using AutoMapper;
using ShoesShop.API.Features.Brands;
using ShoesShop.API.Features.Categories;
using ShoesShop.API.Infrastructure.Database.Entities;

namespace ShoesShop.API.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ==========================================
            // CATEGORY MAPPINGS
            // ==========================================
            // Từ Entity ra DTO
            CreateMap<Category, CategoryResponseDto>();
            // Từ DTO vào Entity
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            // ==========================================
            // BRAND MAPPINGS
            // ==========================================
            CreateMap<Brand, BrandResponseDto>();
            CreateMap<CreateBrandDto, Brand>();
            CreateMap<UpdateBrandDto, Brand>();
        }
    }
}