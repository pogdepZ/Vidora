using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;

namespace Vidora.Core.Interfaces.Api
{
    public interface IMovieApiService
    {
        // --- Quản lý Thể loại (Genres) ---
        Task<Result<List<GenreResult>>> GetGenresAsync(string token);
        Task<Result<GenreResult>> CreateGenreAsync(string token, string name);

        // --- Quản lý Diễn viên (Actors) ---
        Task<Result<List<MemberResult>>> GetMembersAsync(string token);
        Task<Result<MemberResult>> CreateActorAsync(string token, string name);

        // --- Quản lý Phim (Movies) ---
        Task<Result<bool>> CreateMovieAsync(string token, object movieData);
        Task<Result<bool>> UpdateMovieAsync(string token, int movieId, object movieData);
        Task<Result<MoviePaginationResult>> GetAdminMoviesAsync(
            string token,
            int page,
            int limit,
            string? title = null,
            int? genreId = null,
            int? releaseYear = null);
        Task<Result<bool>> ToggleDeleteMovieAsync(string token, int movieId);
        Task<Result<MovieDetailResult>> GetMovieDetailAsync(string token, int movieId);
    }
}
