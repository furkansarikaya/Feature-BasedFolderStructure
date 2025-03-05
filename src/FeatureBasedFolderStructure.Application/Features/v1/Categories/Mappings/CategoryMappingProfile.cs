using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.CreateCategory;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.UpdateCategory;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.DTOs;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<CreateCategoryCommand, Category>().ReverseMap();
        CreateMap<UpdateCategoryCommand, Category>().ReverseMap();
    }
}