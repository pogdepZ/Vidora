using System;

namespace Vidora.Core.Contracts.Results;

/// <summary>
/// Result model for Order
/// GET /api/orders/all
/// </summary>
public class OrderResult
{
    public int OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int PlanId { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal FinalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // User info (flat from API)
    public string UserFullName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    // Plan info (flat from API)
    public string PlanName { get; set; } = string.Empty;
    public decimal PlanPrice { get; set; }
    public int Durations { get; set; }
    public string? Description { get; set; }

    // Discount info
    public int? DiscountId { get; set; }
    public decimal? DiscountAmount { get; set; }

    // Computed properties
    public string AmountDisplay => $"{Amount:N0} VND";
    public string FinalAmountDisplay => $"{FinalAmount:N0} VND";
    public string DiscountAmountDisplay => DiscountAmount.HasValue ? $"-{DiscountAmount.Value:N0} VND" : "None";

    public string StatusDisplay => Status?.ToUpper() switch
    {
        "COMPLETED" => "Completed",
        "PAID" => "Paid",
        "PENDING" => "Pending",
        "FAILED" => "Failed",
        "CANCELLED" => "Cancelled",
        _ => Status ?? "N/A"
    };

    public string StatusColor => Status?.ToUpper() switch
    {
        "COMPLETED" or "PAID" => "Success",
        "PENDING" => "Warning",
        "FAILED" or "CANCELLED" => "Error",
        _ => "Default"
    };

    public string OrderTypeDisplay => OrderType switch
    {
        "New" => "New Subscription",
        "Renewal" => "Renewal",
        _ => OrderType
    };

    public string PaidAtDisplay => PaidAt?.ToString("dd/MM/yyyy HH:mm") ?? "Not Paid";

    public string UserDisplay => $"{UserFullName} ({UserEmail})";
}
