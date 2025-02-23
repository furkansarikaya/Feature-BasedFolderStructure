using FeatureBasedFolderStructure.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();
app.ConfigureApplication();
await app.RunAsync();