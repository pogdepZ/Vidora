using System;
namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

public record PromoData
{
    public int DiscountId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string DiscountType { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public decimal MinOrderValue { get; init; }
    public decimal? MaxDiscount { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public DateTime CreatedAt { get; init; }
}