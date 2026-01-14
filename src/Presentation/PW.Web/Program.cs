using PW.Identity;
using PW.Persistence;
using PW.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProjectServices(builder);

var app = builder.Build();

await app.InitialiseDatabaseAsync();
await app.InitialiseIdentityAsync();

app.ConfigurePipeline();
app.ConfigureRoutes();

app.Run();
