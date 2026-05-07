using Microsoft.Extensions.DependencyInjection;
using SmartReminder.Application.Interfaces;
using SmartReminder.Application.Services;

namespace SmartReminder.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITaskService, TaskService>();

        return services;
    }
}