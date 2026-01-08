using System;
using System.Collections.Generic;

namespace Vidora.Core.Contracts.Results;

/// <summary>
/// Result cho chi ti?t user bao g?m subscriptions và orders
/// </summary>
public record UserDetailResult(
    AdminUserResult User,
    IReadOnlyList<UserSubscriptionResult> Subscriptions,
    IReadOnlyList<UserOrderResult> Orders
);

/// <summary>
/// Result cho subscription c?a user
/// </summary>
public record UserSubscriptionResult(
    int SubscriptionId,
    string PlanName,
    DateTime StartDate,
    DateTime EndDate,
    string Status
)
{
    public string StatusDisplayText => Status?.ToUpper() switch
    {
        "ACTIVE" => "?ang ho?t ??ng",
        "EXPIRED" => "?ã h?t h?n",
        "CANCELLED" => "?ã h?y",
        _ => Status ?? "N/A"
    };
}

/// <summary>
/// Result cho order c?a user
/// </summary>
public record UserOrderResult(
    int OrderId,
    decimal Amount,
    string PaymentMethod,
    string Status,
    DateTime CreatedAt
)
{
    public string AmountDisplay => $"{Amount:N0} VND";
    
    public string StatusDisplayText => Status?.ToUpper() switch
    {
        "COMPLETED" => "Hoàn thành",
        "PENDING" => "?ang x? lý",
        "FAILED" => "Th?t b?i",
        "CANCELLED" => "?ã h?y",
        _ => Status ?? "N/A"
    };
}
