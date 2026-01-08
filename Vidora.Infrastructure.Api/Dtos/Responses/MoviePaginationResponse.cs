using System.Collections.Generic;
using Vidora.Infrastructure.Api.Dtos.Responses.Metas;

namespace Vidora.Infrastructure.Api.Dtos.Responses;

public class MoviePaginationResponse
{
    public bool Success { get; set; }

    public List<AdminMovieDto> Data { get; set; } = [];
    public PaginationMeta Pagination { get; set; }
}

public class AdminMovieDto
{

    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public List<string> Genres { get; set; } = new();
    public string GenresText => string.Join(", ", Genres);
}
