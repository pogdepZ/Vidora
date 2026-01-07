using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

/// <summary>
/// UseCase ?? l?y chi ti?t user bao g?m subscriptions và orders
/// </summary>
public class GetUserDetailUseCase
{
    private readonly IUserApiService _userApiService;
    private readonly ISessionStateService _sessionService;

    public GetUserDetailUseCase(IUserApiService userApiService, ISessionStateService sessionService)
    {
        _userApiService = userApiService;
        _sessionService = sessionService;
    }

    public async Task<Result<UserDetailResult>> ExecuteAsync(int userId)
    {
        var token = _sessionService.CurrentSession?.AccessToken?.Token?.Trim('"');

        if (string.IsNullOrEmpty(token))
            return Result.Failure<UserDetailResult>("Phiên ??ng nh?p không h?p l?.");

        return await _userApiService.GetUserDetailAsync(token, userId);
    }
}
