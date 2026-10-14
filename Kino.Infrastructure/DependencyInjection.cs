using Kino.Application.Interfaces;
using Kino.Domain.Interfaces;
using Kino.Infrastructure.Persistence;
using Kino.Infrastructure.Repositories;
using Kino.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kino.Infrastructure;

// реєстрація всього, що стосується бази даних і файлів.
// Web викликає один метод і не знає ні про EF Core, ні про рядок підключення
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string webRootPath)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        // реалізація інтерфейсу з Domain
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // реалізація інтерфейсу з Application
        services.AddSingleton(new PosterStorageOptions { WebRootPath = webRootPath });
        services.AddScoped<IPosterStorage, PosterStorage>();

        return services;
    }

    // створює базу (разом із початковими даними), якщо її ще немає
    public static IServiceProvider InitializeDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        return services;
    }
}
