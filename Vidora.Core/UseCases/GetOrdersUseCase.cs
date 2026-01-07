using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

/// <summary>
/// UseCase ?? l?y danh sách Orders có pagination và filter
/// </summary>
public class GetOrdersUseCase
{
    private readonly ISubscriptionApiService _subscriptionApiService;
    private readonly ISessionStateService _sessionService;

    public GetOrdersUseCase(
        ISubscriptionApiService subscriptionApiService,
        ISessionStateService sessionService)
    {
        _subscriptionApiService = subscriptionApiService;
        _sessionService = sessionService;
    }

    /// <summary>
    /// L?y danh sách orders v?i các filter
    /// </summary>
    /// <param name="page">S? trang (1-based)</param>
    /// <param name="limit">S? items m?i trang</param>
    /// <param name="search">Tìm ki?m theo user.full_name, user.email, plan.name</param>
    /// <param name="status">Filter theo tr?ng thái: COMPLETED, PENDING, FAILED</param>
    /// <param name="planId">Filter theo planId (optional)</param>
    public async Task<Result<OrderPaginationResult>> ExecuteAsync(
        int page = 1,
        int limit = 10,
        string? search = null,
        string? status = null,
        int? planId = null)
    {
        var token = _sessionService.CurrentSession?.AccessToken?.Token?.Trim('"');

        if (string.IsNullOrEmpty(token))
            return Result.Failure<OrderPaginationResult>("Phiên ??ng nh?p không h?p l?.");

        return await _subscriptionApiService.GetOrdersAsync(token, page, limit, search, status, planId);
    }
}
