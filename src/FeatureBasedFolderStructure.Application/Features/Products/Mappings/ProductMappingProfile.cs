using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.Products.Commands.CreateProduct;
using FeatureBasedFolderStructure.Application.Features.Products.Commands.UpdateProduct;
using FeatureBasedFolderStructure.Application.Features.Products.DTOs;
using FeatureBasedFolderStructure.Domain.Entities;

namespace FeatureBasedFolderStructure.Application.Features.Products.Mappings;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductCommand, Product>();
        CreateMap<UpdateProductCommand, Product>();
    }
}