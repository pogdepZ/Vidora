using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;

namespace Vidora.Core.Interfaces.Api;

/// <summary>
/// Interface cho User API Service
/// </summary>
public interface IUserApiService
{
    /// <summary>
    /// L?y danh sách users có pagination và filter
    /// GET /api/users
    /// </summary>
    Task<Result<UserPaginationResult>> GetUsersAsync(
        string token,
        int page,
        int limit,
        string? fullName = null,
        string? email = null,
        string? username = null,
        string? role = null,
        string? status = null);

    /// <summary>
    /// L?y chi ti?t user bao g?m subscriptions và orders
    /// GET /api/users/{id}
    /// </summary>
    Task<Result<UserDetailResult>> GetUserDetailAsync(string token, int userId);

    /// <summary>
    /// Khóa/M? khóa user (Toggle Status)
    /// PUT /api/users/{id}/status
    /// </summary>
    Task<Result<string>> ToggleUserStatusAsync(string token, int userId);
}
