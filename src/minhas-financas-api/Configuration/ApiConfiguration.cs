using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace minhas_financas_api.Configuration
{
    public static class ApiConfiguration
    {
        public static IServiceCollection AddWebApiConfiguration(this IServiceCollection services)
        {
            services.AddControllers();

            return services;
        }

        public static IApplicationBuilder UseWebApiConfiguration(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            
            app.UseRouting();

            return app;
        }
    }
}
