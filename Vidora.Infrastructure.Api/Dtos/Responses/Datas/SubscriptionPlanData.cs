namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

public record SubscriptionPlanData(
    int PlanId,
    string Name,
    string Price,
    int Duration,
    string? Descrition
    );