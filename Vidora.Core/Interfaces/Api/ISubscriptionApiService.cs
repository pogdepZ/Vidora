using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Results;

namespace Vidora.Core.Interfaces.Api;

/// <summary>
/// Interface cho Subscription/Promo/Order API Service
/// </summary>
public interface ISubscriptionApiService
{
    /// <summary>
    /// L?y danh sách Subscription Plans
    /// GET /api/subscriptions/plans
    /// </summary>
    Task<Result<IReadOnlyList<SubscriptionPlanResult>>> GetPlansAsync();

    /// <summary>
    /// L?y danh sách Promos có pagination
    /// GET /api/promos
    /// </summary>
    Task<Result<PromoPaginationResult>> GetPromosAsync(int page, int limit);

    /// <summary>
    /// T?o Promo m?i
    /// POST /api/promos
    /// </summary>
    Task<Result<bool>> CreatePromoAsync(CreatePromoCommand command);
    /// <summary>
    /// L?y danh sách Orders có pagination và filter
    /// GET /api/orders/all
    /// </summary>
    /// <param name="page">S? trang</param>
    /// <param name="limit">S? items m?i trang</param>
    /// <param name="search">Tìm ki?m theo user.full_name, user.email, plan.name</param>
    /// <param name="status">Filter theo tr?ng thái: COMPLETED, PENDING, FAILED</param>
    /// <param name="planId">Filter theo planId</param>
    Task<Result<OrderPaginationResult>> GetOrdersAsync(
        int page,
        int limit,
        string? search = null,
        string? status = null,
        int? planId = null);
}
