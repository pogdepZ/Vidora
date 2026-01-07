using CSharpFunctionalExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

/// <summary>
/// UseCase ?? l?y danh sách Subscription Plans
/// </summary>
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
        var token = _sessionService.CurrentSession?.AccessToken?.Token?.Trim('"');

        if (string.IsNullOrEmpty(token))
            return Result.Failure<IReadOnlyList<SubscriptionPlanResult>>("Phiên ??ng nh?p không h?p l?.");

        return await _subscriptionApiService.GetPlansAsync(token);
    }
}
