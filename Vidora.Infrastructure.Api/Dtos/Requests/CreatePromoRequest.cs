using System;

namespace Vidora.Infrastructure.Api.Dtos.Requests;

/// <summary>
/// Request DTO ?? t?o Promo m?i
/// POST /api/promos
/// </summary>
internal record CreatePromoRequestDto(
    string Code,
    string DiscountType,
    decimal Value,
    decimal MinOrderValue,
    decimal? MaxDiscount,
    DateTime StartDate,
    DateTime EndDate
);
