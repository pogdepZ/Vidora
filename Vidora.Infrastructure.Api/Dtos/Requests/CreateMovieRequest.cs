using System.Collections.Generic;

namespace Vidora.Infrastructure.Api.Dtos.Requests;

public class CreateMovieRequest
{
    public string Title { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string DirectorName { get; set; } = string.Empty;

    public List<string> Genres { get; set; } = new();
    public List<string> Actors { get; set; } = new();
}
