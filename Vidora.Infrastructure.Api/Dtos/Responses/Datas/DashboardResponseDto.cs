using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

public record DashboardResponseDto
{
    public int TotalUsers { get; set; }

    [JsonPropertyName("totalTodayNewUsers")]
    public int TotalTodayNewUsers { get; set; }

    [JsonPropertyName("newUsers")]
    public List<UserDto> NewUsers { get; set; }

    [JsonPropertyName("todayViews")]
    public int TodayViews { get; set; }

    [JsonPropertyName("totalMovies")]
    public int TotalMovies { get; set; }

    [JsonPropertyName("MostWatchedMovies")] // PascalCase theo đúng JSON
    public List<MovieDto> MostWatchedMovies { get; set; }

    [JsonPropertyName("HighestRatedMovies")] // PascalCase theo đúng JSON
    public List<MovieDto> HighestRatedMovies { get; set; }

    [JsonPropertyName("revenueByDayinCurrentMonth")]
    public RevenueChartDto RevenueByDayinCurrentMonth { get; set; }
}

public record MovieDto(
    int MovieId,
    string Title,
    string? Description,
    int ReleaseYear,
    string? PosterUrl,
    string? TrailerUrl,
    string? MovieUrl,
    string? BannerUrl,
    int? TotalViews, // Có thể null tùy API
    double? AvgRating,
    int? RatingCount,
    bool IsDeleted
);

public class UserDto
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }
}

public class RevenueChartDto
{
    public List<string> Labels { get; set; }
    public List<double> Data { get; set; }
}
