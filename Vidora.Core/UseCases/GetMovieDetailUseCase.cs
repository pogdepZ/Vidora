using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

public class GetMovieDetailUseCase
{
    private readonly IMovieApiService _movieApiService;
    private readonly ISessionStateService _sessionService;

    public GetMovieDetailUseCase(IMovieApiService movieApiService, ISessionStateService sessionService)
    {
        _movieApiService = movieApiService;
        _sessionService = sessionService;
    }

    public async Task<Result<MovieDetailResult>> ExecuteAsync(int movieId)
    {
        return await _movieApiService.GetMovieDetailAsync(movieId);
    }
}
