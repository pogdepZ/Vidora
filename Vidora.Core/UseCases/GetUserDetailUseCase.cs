using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

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
        return await _userApiService.GetUserDetailAsync(userId);
    }
}
