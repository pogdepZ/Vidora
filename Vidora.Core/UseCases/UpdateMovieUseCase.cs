using CSharpFunctionalExtensions;
using System.Linq;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

public class UpdateMovieUseCase
{
    private readonly IMovieApiService _movieApiService;
    private readonly ISessionStateService _sessionService;

    public UpdateMovieUseCase(IMovieApiService movieApiService, ISessionStateService sessionService)
    {
        _movieApiService = movieApiService;
        _sessionService = sessionService;
    }

    public async Task<Result<bool>> ExecuteAsync(UpdateMovieCommand command)
    {
        var token = _sessionService.CurrentSession?.AccessToken?.Token?.Trim('"');
        if (string.IsNullOrEmpty(token))
            return Result.Failure<bool>("Phiên ??ng nh?p h?t h?n.");

        // Build the movie data object matching API format
        var movieData = new
        {
            title = command.Title,
            description = command.Description,
            releaseYear = command.ReleaseYear,
            posterUrl = command.PosterUrl,
            bannerUrl = command.BannerUrl,
            trailerUrl = command.TrailerUrl,
            movieUrl = command.MovieUrl,
            genres = command.GenreIds.Select(id => new { genresId = id }).ToList(),
            castAndCrew = command.CastAndCrew.Select(c => new { memberId = c.MemberId, role = c.Role }).ToList()
        };

        return await _movieApiService.UpdateMovieAsync(command.MovieId, movieData);
    }
}
