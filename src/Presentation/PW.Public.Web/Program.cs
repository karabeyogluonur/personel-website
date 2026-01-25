using PW.Public.Web.Framework.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPublicProjectServices(builder);

var app = builder.Build();


app.ConfigurePublicPipeline();
app.ConfigurePublicRoutes();

app.Run();
