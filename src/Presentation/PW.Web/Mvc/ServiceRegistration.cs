using PW.Application;
using PW.Persistence;

namespace PW.Web.Mvc
{
    public static class ServiceRegistration
    {
        public static void AddLayerServices(this IHostApplicationBuilder builder)
        {
            builder.AddApplicationServices();
            builder.AddPersistenceServices();
        }
        public static void AddControllersService(this IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }
    }
}
