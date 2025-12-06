using PW.Identity;
using PW.Persistence;
using PW.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebLayerServices();
builder.Services.AddOrchestratorServices();
builder.Services.AddApplicationInfrastructure(builder);
var localizationConfig = builder.Services.AddDatabaseLocalization();
builder.Services.ConfigureCustomApplicationCookie();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    await app.InitialiseDatabaseAsync();
    await app.InitialiseIdentityAsync();
}
else
    app.UseDeveloperExceptionPage();

app.ConfigurePipeline();
app.ConfigureRoutes(localizationConfig.CultureConstraint);

app.Run();
