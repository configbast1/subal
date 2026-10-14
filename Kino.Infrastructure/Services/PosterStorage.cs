using Kino.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Kino.Infrastructure.Services;

// шлях до wwwroot передає Web під час реєстрації,
// тому Infrastructure не залежить від ASP.NET Core (IWebHostEnvironment)
public class PosterStorageOptions
{
    public string WebRootPath { get; set; } = string.Empty;
}

// реалізація IPosterStorage з Application: зберігає файли постерів на диск
public class PosterStorage : IPosterStorage
{
    private const string UploadsFolder = "/images/uploads/";
    private const string PlaceholderPoster = "/images/poster-placeholder.svg";

    private readonly string _webRoot;
    private readonly ILogger<PosterStorage> _logger;

    public PosterStorage(PosterStorageOptions options, ILogger<PosterStorage> logger)
    {
        _webRoot = options.WebRootPath;
        _logger = logger;
    }

    public string PlaceholderPath => PlaceholderPoster;

    public async Task<string?> SaveAsync(Stream? content, string? originalFileName)
    {
        if (content is null)
        {
            return null;
        }

        var extension = Path.GetExtension(originalFileName ?? string.Empty).ToLowerInvariant();
        var uploadsDirectory = Path.Combine(_webRoot, "images", "uploads");
        Directory.CreateDirectory(uploadsDirectory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsDirectory, fileName);

        await using (content)
        await using (var stream = File.Create(fullPath))
        {
            await content.CopyToAsync(stream);
        }

        _logger.LogInformation("Збережено постер {FileName}", fileName);

        return $"{UploadsFolder}{fileName}";
    }

    public void Delete(string? posterPath)
    {
        if (string.IsNullOrWhiteSpace(posterPath))
        {
            return;
        }

        // видаляємо тільки те, що завантажив користувач; початкові постери не чіпаємо
        if (!posterPath.StartsWith(UploadsFolder, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var relative = posterPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_webRoot, relative);

        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("Видалено постер {PosterPath}", posterPath);
            }
        }
        catch (IOException ex)
        {
            _logger.LogWarning(ex, "Не вдалося видалити постер {PosterPath}", posterPath);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Немає прав на видалення постера {PosterPath}", posterPath);
        }
    }
}
