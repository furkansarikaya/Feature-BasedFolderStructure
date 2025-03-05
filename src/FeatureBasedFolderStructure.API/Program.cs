using FeatureBasedFolderStructure.API.Extensions;
using FeatureBasedFolderStructure.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();
app.ConfigureApplication();
using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}
await app.RunAsync();