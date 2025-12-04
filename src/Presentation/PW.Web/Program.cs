using PW.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebLayerServices();
builder.Services.AddApplicationInfrastructure(builder);

var localizationConfig = builder.Services.AddDatabaseLocalization();

var app = builder.Build();
app.ConfigurePipeline();
app.ConfigureRoutes(localizationConfig.CultureConstraint);

app.Run();
