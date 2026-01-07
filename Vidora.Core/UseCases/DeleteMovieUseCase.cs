using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

public class DeleteMovieUseCase
{
    private readonly IMovieApiService _movieApiService;
    private readonly ISessionStateService _sessionService;

    public DeleteMovieUseCase(IMovieApiService movieApiService, ISessionStateService sessionService)
    {
        _movieApiService = movieApiService;
        _sessionService = sessionService;
    }

    public async Task<Result<bool>> ExecuteAsync(int movieId)
    {
        // 1. Lấy token giống như GetMovieDetailUseCase
        var token = _sessionService.CurrentSession?.AccessToken?.Token;

        if (string.IsNullOrEmpty(token))
            return Result.Failure<bool>("Phiên đăng nhập hết hạn.");

        if (movieId <= 0)
            return Result.Failure<bool>("ID phim không hợp lệ.");

        // 2. Gọi API Service thực hiện toggle delete
        return await _movieApiService.ToggleDeleteMovieAsync(movieId);
    }
}