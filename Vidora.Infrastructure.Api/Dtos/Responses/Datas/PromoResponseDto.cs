using System;
using System.Collections.Generic;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

/// <summary>
/// DTO cho response danh sách Promos có pagination
/// GET /api/promos
/// </summary>
internal record PromoResponseDto
{
    public bool Success { get; init; }
    public List<PromoItemDto> Data { get; init; } = new();
    public PaginationDto Pagination { get; init; } = new();
}

/// <summary>
/// DTO cho t?ng Promo item
/// </summary>
internal record PromoItemDto
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

/// <summary>
/// DTO cho response t?o Promo m?i
/// POST /api/promos
/// </summary>
internal record CreatePromoResponseDto
{
    public bool Success { get; init; }
    public PromoItemDto? Data { get; init; }
    public string? Message { get; init; }
}
