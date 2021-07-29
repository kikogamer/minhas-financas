using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using minhas_financas.business.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace minhas_financas.api.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<INotificador, Notificador>();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }
    }
}
