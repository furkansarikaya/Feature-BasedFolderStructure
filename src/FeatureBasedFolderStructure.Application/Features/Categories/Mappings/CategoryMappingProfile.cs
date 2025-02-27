using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.Categories.Commands.CreateCategory;
using FeatureBasedFolderStructure.Application.Features.Categories.Commands.UpdateCategory;
using FeatureBasedFolderStructure.Application.Features.Categories.DTOs;
using FeatureBasedFolderStructure.Domain.Entities;

namespace FeatureBasedFolderStructure.Application.Features.Categories.Mappings;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<CreateCategoryCommand, Category>().ReverseMap();
        CreateMap<UpdateCategoryCommand, Category>().ReverseMap();
    }
}