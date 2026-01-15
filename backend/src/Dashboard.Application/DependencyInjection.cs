using Dashboard.Application.Services;
using Dashboard.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Dashboard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IServiceQueryService, ServiceQueryService>();
        services.AddScoped<IIncidentService, IncidentService>();
        return services;
    }
}
