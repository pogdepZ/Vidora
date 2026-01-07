using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Results;

namespace Vidora.Core.Interfaces.Api;

public interface ISubscriptionApiService
{
    Task<Result<IReadOnlyList<SubscriptionPlanResult>>> GetPlansAsync();

    Task<Result<PromoPaginationResult>> GetPromosAsync(int page, int limit);

    Task<Result<bool>> CreatePromoAsync(CreatePromoCommand command);

    Task<Result<OrderPaginationResult>> GetOrdersAsync(
        int page,
        int limit,
        string? search = null,
        string? status = null,
        int? planId = null);
}
