using System.Collections.Generic;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

public record MovieData(
    int MovieId,
    string Title,
    string? Description,
    int ReleaseYear,
    string? PosterUrl,
    string? TrailerUrl,
    string? MovieUrl,
    string? BannerUrl,
    int? TotalViews,
    double? AvgRating,
    int? RatingCount,
    List<string>? genres,
    List<GenreData>? Genres,
    List<MemberData>? Actors,
    bool? IsDeleted
);

