using System;

namespace Vidora.Core.Contracts.Results;

/// <summary>
/// Result model cho Promo/Discount
/// GET /api/promos
/// </summary>
public class PromoResult
{
    public int DiscountId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal MinOrderValue { get; set; }
    public decimal? MaxDiscount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; }

    // Computed properties
    public string DiscountTypeDisplay => DiscountType switch
    {
        "fixed_amount" => "Cố định",
        "percentage" => "Phần trăm",
        _ => DiscountType
    };

    public string ValueDisplay => DiscountType == "percentage" 
        ? $"{Value}%" 
        : $"{Value:N0} VND";

    public string MinOrderValueDisplay => $"{MinOrderValue:N0} VND";

    public string MaxDiscountDisplay => MaxDiscount.HasValue 
        ? $"{MaxDiscount.Value:N0} VND" 
        : "Không giới hạn";

    public string StartDateDisplay => StartDate.ToString("dd/MM/yyyy");
    public string EndDateDisplay => EndDate.ToString("dd/MM/yyyy");
    public string CreatedAtDisplay => CreatedAt.ToString("dd/MM/yyyy HH:mm");

    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;

    public string StatusDisplay => IsActive ? "Đang hoạt động" : 
        (DateTime.UtcNow < StartDate ? "Chưa bắt đầu" : "Đã hết hạn");
}
