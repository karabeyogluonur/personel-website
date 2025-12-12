using Microsoft.Extensions.Localization;
using PW.Identity;
using PW.Persistence;
using PW.Web.Extensions;
using PW.Web.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProjectServices(builder);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
    await app.InitialiseIdentityAsync();
}

app.ConfigurePipeline();
app.ConfigureRoutes();

app.Run();
