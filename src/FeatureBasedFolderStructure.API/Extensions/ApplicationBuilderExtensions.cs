using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace FeatureBasedFolderStructure.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ConfigureApplication(this WebApplication app)
    {
        var apiVersionDescriptionProvider = app.Services
            .GetRequiredService<IApiVersionDescriptionProvider>();
            app.MapOpenApi();
            app.UseSwagger(options =>
            {
                options.SerializeAsV2 = true;
                options.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.DocumentTitle = $"Feature-Based Folder Structure API {description.GroupName.ToUpperInvariant()} - {app.Environment.EnvironmentName}";
                    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    options.SwaggerEndpoint(url: $"/swagger/{description.GroupName}/swagger.json",
                        name: description.GroupName.ToUpperInvariant());
                    
                }
        
            });
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}