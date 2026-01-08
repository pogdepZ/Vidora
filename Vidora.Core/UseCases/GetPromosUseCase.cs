using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

public class GetPromosUseCase
{
    private readonly ISubscriptionApiService _subscriptionApiService;
    private readonly ISessionStateService _sessionService;

    public GetPromosUseCase(
        ISubscriptionApiService subscriptionApiService,
        ISessionStateService sessionService)
    {
        _subscriptionApiService = subscriptionApiService;
        _sessionService = sessionService;
    }

    public async Task<Result<PromoPaginationResult>> ExecuteAsync(int page, int limit = 10)
    {
        return await _subscriptionApiService.GetPromosAsync(page, limit);
    }
}
