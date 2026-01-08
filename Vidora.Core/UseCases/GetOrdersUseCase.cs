using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

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

    public async Task<Result<OrderPaginationResult>> ExecuteAsync(
        int page = 1,
        int limit = 10,
        string? search = null,
        string? status = null,
        int? planId = null)
    {
        return await _subscriptionApiService.GetOrdersAsync(page, limit, search, status, planId);
    }
}
