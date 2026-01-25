using PW.Identity;
using PW.Persistence;
using PW.Admin.Web.Framework.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAdminProjectServices(builder);

var app = builder.Build();

await app.InitialiseDatabaseAsync();
await app.InitialiseIdentityAsync();

app.ConfigureAdminPipeline();
app.ConfigureAdminRoutes();

app.Run();
