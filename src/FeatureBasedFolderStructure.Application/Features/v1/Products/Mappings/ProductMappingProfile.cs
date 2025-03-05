using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.CreateProduct;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.UpdateProduct;
using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using FeatureBasedFolderStructure.Domain.Common.Interfaces;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<CreateProductCommand, Product>().ReverseMap();
        CreateMap<UpdateProductCommand, Product>().ReverseMap();
        
        CreateMap<IPaginate<Product>, ProductListDto>().ReverseMap();
    }
}