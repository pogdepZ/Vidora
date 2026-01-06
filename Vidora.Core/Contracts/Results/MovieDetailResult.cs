using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vidora.Core.Contracts.Results;

public record MovieDetailResult(
    int MovieId,
    string Title,
    string Description,
    int ReleaseYear,
    string PosterUrl,
    string BannerUrl,
    string TrailerUrl,
    string MovieUrl,
    double AvgRating,
    List<string> Genres,
    List<MovieMemberResult> Actors
)
{
    // Thuộc tính tính toán để lấy nhanh tên Đạo diễn
    public string DirectorName => Actors
        ?.FirstOrDefault(a => string.Equals(a.Role?.Trim(), "Director", StringComparison.OrdinalIgnoreCase))
        ?.Name ?? "Chưa rõ";
}

public record MovieMemberResult(
    int MemberId,
    string Name,
    string Role
);
