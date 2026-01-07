using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;
using Vidora.Core.Services;

namespace Vidora.Core.UseCases
{
    public class GetMoviesUseCase
    {
        private readonly IMovieApiService _movieApiService;
        private readonly ISessionStateService _sessionService;

        public GetMoviesUseCase(IMovieApiService movieApiService, ISessionStateService sessionService)
        {
            _movieApiService = movieApiService;
            _sessionService = sessionService;
        }

        public async Task<Result<MoviePaginationResult>> ExecuteAsync(
        int page,
        int limit = 10,
        string? title = null,
        int? genreId = null,
        int? releaseYear = null)
        {
            var token = _sessionService.CurrentSession?.AccessToken?.Token;
            if (string.IsNullOrEmpty(token)) return Result.Failure<MoviePaginationResult>("Phiên đăng nhập hết hạn.");

            return await _movieApiService.GetAdminMoviesAsync(page, limit, title, genreId, releaseYear);
        }
    }
}