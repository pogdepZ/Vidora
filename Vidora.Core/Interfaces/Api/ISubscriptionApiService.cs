using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Results;

namespace Vidora.Core.Interfaces.Api;

/// <summary>
/// Interface cho Subscription/Promo API Service
/// </summary>
public interface ISubscriptionApiService
{
    /// <summary>
    /// L?y danh sách Subscription Plans
    /// GET /api/subscriptions/plans
    /// </summary>
    Task<Result<IReadOnlyList<SubscriptionPlanResult>>> GetPlansAsync(string token);

    /// <summary>
    /// L?y danh sách Promos có pagination
    /// GET /api/promos
    /// </summary>
    Task<Result<PromoPaginationResult>> GetPromosAsync(string token, int page, int limit);

    /// <summary>
    /// T?o Promo m?i
    /// POST /api/promos
    /// </summary>
    Task<Result<PromoResult>> CreatePromoAsync(string token, CreatePromoCommand command);
}
