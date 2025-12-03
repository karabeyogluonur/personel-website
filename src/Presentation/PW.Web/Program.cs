using PW.Web.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersService();
builder.AddLayerServices();

var app = builder.Build();

app.ConfigureMiddleware();
app.ConfigureRouting();

app.Run();
