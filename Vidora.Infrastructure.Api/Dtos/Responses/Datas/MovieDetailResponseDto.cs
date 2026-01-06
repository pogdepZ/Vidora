using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Vidora.Infrastructure.Api.Dtos.Responses;

public class MovieDetailResponseDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public MovieDetailDataDto Data { get; set; }
}

public class MovieDetailDataDto
{
    [JsonPropertyName("movieId")]
    public int MovieId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("releaseYear")]
    public int ReleaseYear { get; set; }

    [JsonPropertyName("posterUrl")]
    public string PosterUrl { get; set; }

    [JsonPropertyName("bannerUrl")]
    public string BannerUrl { get; set; }

    [JsonPropertyName("trailerUrl")]
    public string TrailerUrl { get; set; }

    [JsonPropertyName("movieUrl")]
    public string MovieUrl { get; set; }

    [JsonPropertyName("avgRating")]
    public double? AvgRating { get; set; }

    [JsonPropertyName("genres")]
    public List<string>? Genres { get; set; }

    // Đây là nơi quan trọng nhất để fix lỗi "N/A"
    [JsonPropertyName("actors")]
    public List<MovieMemberDto> Actors { get; set; }
}

public class MovieMemberDto
{
    [JsonPropertyName("memberId")]
    public int MemberId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }
}