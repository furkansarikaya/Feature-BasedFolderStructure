using Scalar.AspNetCore;

namespace FeatureBasedFolderStructure.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ConfigureApplication(this WebApplication app)
    {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("Feature-Based Folder Structure API")
                    .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
            });
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}