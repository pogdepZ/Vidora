using System;

namespace Vidora.Core.Contracts.Commands;

/// <summary>
/// Command ?? t?o Promo m?i
/// POST /api/promos
/// </summary>
public record CreatePromoCommand(
    string Code,
    string DiscountType,
    decimal Value,
    decimal MinOrderValue,
    decimal? MaxDiscount,
    DateTime StartDate,
    DateTime EndDate
);
