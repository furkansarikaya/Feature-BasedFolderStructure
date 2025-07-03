using FS.AspNetCore.ResponseWrapper.Middlewares;
using Scalar.AspNetCore;

namespace FeatureBasedFolderStructure.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ConfigureApplication(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference("/docs", options =>
        {
            options
                .WithTitle("Feature-Based Folder Structure API")
                .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios)
                .AddPreferredSecuritySchemes("BearerAuth")
                .AddHttpAuthentication("BearerAuth", auth =>
                {
                    auth.Token = "ey...";
                })
                .WithPersistentAuthentication();
        });

        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}