using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartReminder.Application.Interfaces;
using SmartReminder.Infrastructure.Persistence;
using SmartReminder.Infrastructure.Repositories;
using SmartReminder.Infrastructure.Security;

namespace SmartReminder.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SmartReminderConnection");

        services.AddDbContext<SmartReminderDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPomodoroRepository, PomodoroRepository>();

        return services;
    }
}