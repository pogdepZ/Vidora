using System;
using System.Collections.Generic;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

/// <summary>
/// DTO cho response danh sách Orders có pagination
/// GET /api/orders/all
/// </summary>
internal record OrderPaginationResponseDto
{
    public bool Success { get; init; }
    public List<OrderItemDto> Data { get; init; } = new();
    public PaginationDto Pagination { get; init; } = new();
}

/// <summary>
/// DTO cho t?ng Order item (flat structure t? API)
/// </summary>
internal record OrderItemDto
{
    public int OrderId { get; init; }
    public int UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string OrderCode { get; init; } = string.Empty;
    public string OrderType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime? PaidAt { get; init; }
    public int PlanId { get; init; }
    public string PlanName { get; init; } = string.Empty;
    public decimal PlanPrice { get; init; }
    public int Durations { get; init; }
    public string? Description { get; init; }
    public int? DiscountId { get; init; }
    public decimal? DiscountAmount { get; init; }
    public decimal FinalAmount { get; init; }
}
