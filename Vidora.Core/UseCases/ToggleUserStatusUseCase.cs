using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

/// <summary>
/// UseCase ?? khóa/m? khóa user (Toggle Status)
/// </summary>
public class ToggleUserStatusUseCase
{
    private readonly IUserApiService _userApiService;
    private readonly ISessionStateService _sessionService;

    public ToggleUserStatusUseCase(IUserApiService userApiService, ISessionStateService sessionService)
    {
        _userApiService = userApiService;
        _sessionService = sessionService;
    }

    /// <summary>
    /// Toggle status c?a user
    /// </summary>
    /// <param name="userId">ID c?a user</param>
    /// <returns>Status m?i c?a user (ACTIVE/LOCKED)</returns>
    public async Task<Result<string>> ExecuteAsync(int userId)
    {
        var token = _sessionService.CurrentSession?.AccessToken?.Token?.Trim('"');

        if (string.IsNullOrEmpty(token))
            return Result.Failure<string>("Phiên ??ng nh?p không h?p l?.");

        return await _userApiService.ToggleUserStatusAsync(userId);
    }
}
