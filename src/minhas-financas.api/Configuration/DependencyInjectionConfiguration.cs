using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using minhas_financas.api.Extensions;
using minhas_financas.business.Interfaces;
using minhas_financas.business.Notificacoes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace minhas_financas.api.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<JwtGeradorToken>();
            
            services.AddScoped<INotificador, Notificador>();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }
    }
}
