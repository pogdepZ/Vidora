using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

public class ToggleUserStatusUseCase
{
    private readonly IUserApiService _userApiService;
    private readonly ISessionStateService _sessionService;

    public ToggleUserStatusUseCase(IUserApiService userApiService, ISessionStateService sessionService)
    {
        _userApiService = userApiService;
        _sessionService = sessionService;
    }

    public async Task<Result<string>> ExecuteAsync(int userId)
    {
        return await _userApiService.ToggleUserStatusAsync(userId);
    }
}
