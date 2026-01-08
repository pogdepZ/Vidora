using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.Interfaces.Api;

namespace Vidora.Core.UseCases;

/// <summary>
/// UseCase ?? t?o Promo m?i
/// </summary>
public class CreatePromoUseCase
{
    private readonly ISubscriptionApiService _subscriptionApiService;
    private readonly ISessionStateService _sessionService;

    public CreatePromoUseCase(
        ISubscriptionApiService subscriptionApiService,
        ISessionStateService sessionService)
    {
        _subscriptionApiService = subscriptionApiService;
        _sessionService = sessionService;
    }

    public async Task<Result<bool>> ExecuteAsync(CreatePromoCommand command)
    {
        var token = _sessionService.CurrentSession?.AccessToken?.Token?.Trim('"');

        if (string.IsNullOrEmpty(token))
            return Result.Failure<bool>("Phiên ??ng nh?p không h?p l?.");

        // Validation
        if (string.IsNullOrWhiteSpace(command.Code))
            return Result.Failure<bool>("Mã gi?m giá không ???c ?? tr?ng.");

        if (command.DiscountType != "fixed_amount" && command.DiscountType != "percentage")
            return Result.Failure<bool>("Lo?i gi?m giá không h?p l?.");

        if (command.Value <= 0)
            return Result.Failure<bool>("Giá tr? gi?m giá ph?i l?n h?n 0.");

        if (command.DiscountType == "percentage" && command.Value > 100)
            return Result.Failure<bool>("Ph?n tr?m gi?m giá không ???c v??t quá 100%.");

        if (command.MinOrderValue < 0)
            return Result.Failure<bool>("Giá tr? ??n hàng t?i thi?u không h?p l?.");

        if (command.StartDate >= command.EndDate)
            return Result.Failure<bool>("Ngày b?t ??u ph?i tr??c ngày k?t thúc.");

        return await _subscriptionApiService.CreatePromoAsync(command);
    }
}
