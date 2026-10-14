namespace Kino.Application.DTO;

// об'єкт передачі даних між Application та рівнем представлення.
// сутність Movie з Domain нагору не піднімається взагалі
public class MovieDto
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

    // вміст завантаженого постера.
    // саме Stream, а не IFormFile - Application не посилається на ASP.NET Core
    public Stream? PosterContent { get; set; }

    public string? PosterFileName { get; set; }
}
