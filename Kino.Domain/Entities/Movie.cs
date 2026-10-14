namespace Kino.Domain.Entities;

// Сутність предметної області (Domain).
// Тут немає жодних атрибутів валідації та типів ASP.NET -
// Domain нічого не знає про веб, EF Core, форми та IFormFile.
// Обмеження полів описані у конфігурації AppDbContext.
public class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? OriginalTitle { get; set; }

    public string Director { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public int ReleaseYear { get; set; }

    public string PosterPath { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Rating { get; set; }
}
