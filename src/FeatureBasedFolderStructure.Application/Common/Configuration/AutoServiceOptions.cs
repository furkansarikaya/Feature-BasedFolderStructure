using Microsoft.Extensions.Configuration;

namespace FeatureBasedFolderStructure.Application.Common.Configuration;

/// <summary>
/// Auto service registration için configuration options.
/// Application layer'da çünkü bu bir application-level configuration.
/// </summary>
public class AutoServiceOptions
{
    public string? Profile { get; set; }
    public bool IsTestEnvironment { get; set; } = false;
    public bool EnableLogging { get; set; } = true;
    public IConfiguration? Configuration { get; set; }
}
