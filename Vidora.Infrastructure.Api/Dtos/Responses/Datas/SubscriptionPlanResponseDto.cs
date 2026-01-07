using System.Collections.Generic;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

/// <summary>
/// DTO cho response danh sách Subscription Plans
/// GET /api/subscriptions/plans
/// </summary>
internal record SubscriptionPlansResponseDto
{
    public bool Success { get; init; }
    public List<SubscriptionPlanDto> Data { get; init; } = new();
}

/// <summary>
/// DTO cho t?ng Subscription Plan
/// </summary>
internal record SubscriptionPlanDto
{
    public int PlanId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Durations { get; init; }
    public string? Description { get; init; }
}
