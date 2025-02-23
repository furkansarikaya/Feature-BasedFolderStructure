using FeatureBasedFolderStructure.API.Common;
using Scalar.AspNetCore;

namespace FeatureBasedFolderStructure.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ConfigureApplication(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference("/docs",options =>
        {
            options
                .WithTitle("Feature-Based Folder Structure API")
                .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
        });

        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}