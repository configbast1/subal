using Kino.Application.Interfaces;
using Kino.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kino.Application;

public static class DependencyInjection
{
    // реєструє лише сервіси бізнес-логіки.
    // Application не знає, хто реалізує IRepository та IPosterStorage — це робить Infrastructure
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMovieService, MovieService>();

        return services;
    }
}
