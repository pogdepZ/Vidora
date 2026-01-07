namespace Vidora.Core.Contracts.Results;

/// <summary>
/// Result model cho Subscription Plan
/// GET /api/subscriptions/plans
/// </summary>
public class SubscriptionPlanResult
{
    public int PlanId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Durations { get; set; }
    public string? Description { get; set; }

    // Computed properties
    public string PriceDisplay => $"{Price:N0} VND";
    public string DurationsDisplay => $"{Durations} ngày";
}
