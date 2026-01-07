using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Entities;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

public class CreateMovieUseCase
{
    private readonly IMovieApiService _movieApiService;
    private readonly ISessionStateService _sessionService;

    public CreateMovieUseCase(IMovieApiService movieApiService, ISessionStateService sessionService)
    {
        _movieApiService = movieApiService;
        _sessionService = sessionService;
    }

    public async Task<Result<bool>> ExecuteAsync(CreateMovieCommand command)
    {
        // 1. Chuyển GenreIds thành format server yêu cầu: [{ genresId: 1 }, { genresId: 2 }]
        var genresPayload = command.GenreIds.Select(id => new { genresId = id }).ToList();

        // 2. Chuyển CastAndCrew thành format server yêu cầu: [{ memberId: 1, role: "Director" }]
        var castPayload = command.CastAndCrew.Select(c => new { memberId = c.MemberId, role = c.Role }).ToList();

        // 3. Post Movie theo đúng Schema JSON Server yêu cầu
        var finalData = new
        {
            title = command.Title,
            description = command.Description,
            releaseYear = command.ReleaseYear,
            posterUrl = command.PosterUrl,
            bannerUrl = command.BannerUrl,
            trailerUrl = command.TrailerUrl,
            movieUrl = command.MovieUrl,
            genres = genresPayload,
            castAndCrew = castPayload
        };

        return await _movieApiService.CreateMovieAsync(finalData);
    }

    public async Task<Result<List<Genre>>> GetGenresAsync()
    {
        var result = await _movieApiService.GetGenresAsync();

        return result.Map(genreResults => genreResults.Select(dto => new Genre
        {
            Id = dto.Id,
            Name = dto.Name
        }).ToList());
    }
}