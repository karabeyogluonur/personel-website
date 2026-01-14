using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PW.Application.Interfaces.Storage;
using PW.Storage.Services;

namespace PW.Storage;

public static class DependencyInjection
{
   public static void AddStorageServices(this IHostApplicationBuilder builder)
   {
      builder.Services.AddScoped<IStorageService, LocalStorageService>();
      builder.Services.AddScoped<IFileProcessorService, FileProcessorService>();
   }
}
