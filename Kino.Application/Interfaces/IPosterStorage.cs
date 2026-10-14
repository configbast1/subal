namespace Kino.Application.Interfaces;

// абстракція сховища постерів.
// реалізація живе в Infrastructure (Kino.Infrastructure.Services.PosterStorage)
public interface IPosterStorage
{
    string PlaceholderPath { get; }

    Task<string?> SaveAsync(Stream? content, string? originalFileName);

    void Delete(string? posterPath);
}
