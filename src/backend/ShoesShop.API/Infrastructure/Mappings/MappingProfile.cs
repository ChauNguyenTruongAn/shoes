using AutoMapper;
using ShoesShop.API.Features.Brands;
using ShoesShop.API.Features.Categories;
using ShoesShop.API.Features.Products;
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

            // ==========================================
            // PRODUCT & VARIANT MAPPINGS
            // ==========================================
            CreateMap<ProductVariant, ProductVariantResponseDto>();
            CreateMap<CreateProductVariantDto, ProductVariant>();

            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name));
                
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
        }
    }
}