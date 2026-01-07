using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

/// <summary>
/// UseCase ?? l?y danh sách users có pagination và filter
/// </summary>
public class GetUsersUseCase
{
    private readonly IUserApiService _userApiService;
    private readonly ISessionStateService _sessionService;

    public GetUsersUseCase(IUserApiService userApiService, ISessionStateService sessionService)
    {
        _userApiService = userApiService;
        _sessionService = sessionService;
    }

    public async Task<Result<UserPaginationResult>> ExecuteAsync(
        int page,
        int limit = 10,
        string? search = null,
        string? email = null,
        string? username = null,
        string? role = null,
        string? status = null)
    {
        return await _userApiService.GetUsersAsync( 
            page, 
            limit, 
            search, 
            email, 
            username, 
            role, 
            status);
    }
}
