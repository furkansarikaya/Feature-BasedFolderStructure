using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FS.AutoServiceDiscovery.Extensions.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Infrastructure.Common.Services;

[ServiceRegistration(ServiceLifetime.Scoped, Order = -1)]
public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}