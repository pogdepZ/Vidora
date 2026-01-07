using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;

namespace Vidora.Core.Interfaces.Api;

public interface IUserApiService
{
    Task<Result<UserPaginationResult>> GetUsersAsync(
        int page,
        int limit,
        string? search = null,
        string? email = null,
        string? username = null,
        string? role = null,
        string? status = null);

    Task<Result<UserDetailResult>> GetUserDetailAsync(int userId);

    Task<Result<string>> ToggleUserStatusAsync(int userId);
}
