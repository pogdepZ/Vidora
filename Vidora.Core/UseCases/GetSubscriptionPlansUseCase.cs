using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

public class GetSubscriptionPlansUseCase
{
    private readonly ISubscriptionApiService _subscriptionApiService;
    private readonly ISessionStateService _sessionService;

    public GetSubscriptionPlansUseCase(
        ISubscriptionApiService subscriptionApiService,
        ISessionStateService sessionService)
    {
        _subscriptionApiService = subscriptionApiService;
        _sessionService = sessionService;
    }

    public async Task<Result<IReadOnlyList<SubscriptionPlanResult>>> ExecuteAsync()
    {
        return await _subscriptionApiService.GetPlansAsync();
    }
}
