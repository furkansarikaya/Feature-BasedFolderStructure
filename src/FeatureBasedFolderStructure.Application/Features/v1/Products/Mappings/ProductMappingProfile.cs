using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.CreateProduct;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.UpdateProduct;
using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Common.Paging;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<CreateProductCommand, Product>().ReverseMap();
        CreateMap<UpdateProductCommand, Product>().ReverseMap();
        
        CreateMap<IPaginate<Product>, PagedResult<ProductDto>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.Index))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.Size))
            .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.Pages))
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.Count))
            .ForMember(dest => dest.HasNextPage, opt => opt.MapFrom(src => src.HasNext))
            .ForMember(dest => dest.HasPreviousPage, opt => opt.MapFrom(src => src.HasPrevious))
            .ReverseMap();
    }
}