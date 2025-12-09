using PW.Identity;
using PW.Persistence;
using PW.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProjectServices(builder);
var localizationConfig = builder.Services.AddDatabaseLocalization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
    await app.InitialiseIdentityAsync();
}

app.ConfigurePipeline();
app.ConfigureRoutes(localizationConfig.CultureConstraint);

app.Run();
