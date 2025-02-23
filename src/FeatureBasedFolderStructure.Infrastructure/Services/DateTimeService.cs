using FeatureBasedFolderStructure.Application.Common.Interfaces;

namespace FeatureBasedFolderStructure.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}