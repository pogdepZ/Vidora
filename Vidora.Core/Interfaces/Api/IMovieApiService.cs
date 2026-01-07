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
        Task<Result<List<GenreResult>>> GetGenresAsync();
        Task<Result<GenreResult>> CreateGenreAsync(string name);

        // --- Quản lý Diễn viên (Actors) ---
        Task<Result<List<MemberResult>>> GetMembersAsync();
        Task<Result<MemberResult>> CreateActorAsync(string name);

        // --- Quản lý Phim (Movies) ---
        Task<Result<bool>> CreateMovieAsync(object movieData);
        Task<Result<bool>> UpdateMovieAsync(int movieId, object movieData);
        Task<Result<MoviePaginationResult>> GetAdminMoviesAsync(
            int page,
            int limit,
            string? title = null,
            int? genreId = null,
            int? releaseYear = null);
        Task<Result<bool>> ToggleDeleteMovieAsync(int movieId);
        Task<Result<MovieDetailResult>> GetMovieDetailAsync(int movieId);
    }
}
