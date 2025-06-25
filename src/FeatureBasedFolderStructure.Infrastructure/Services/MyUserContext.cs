using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FS.EntityFramework.Library;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Infrastructure.Services;

public class MyUserContext(IServiceProvider serviceProvider) : IUserContext
{
    public string? CurrentUser
    {
        get
        {
            var currentUser = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ICurrentUserService>();
            return currentUser.UserId;
        }
    }
}