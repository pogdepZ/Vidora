using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vidora.Infrastructure.Api.Dtos.Responses;

public class MoviePaginationResponseDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public List<AdminMovieDto> Data { get; set; } = [];

    [JsonPropertyName("pagination")]
    public PaginationDto Pagination { get; set; } = new();
}

public class AdminMovieDto
{

    [JsonPropertyName("movieId")]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public List<string> Genres { get; set; } = new();
    public string GenresText => string.Join(", ", Genres);
}

public class PaginationDto
{
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrev { get; set; }
}