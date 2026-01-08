using System;

namespace Vidora.Infrastructure.Api.Dtos.Requests;

public record CreatePromoRequestDto(
    string Code,
    string DiscountType,
    decimal Value,
    decimal MinOrderValue,
    decimal? MaxDiscount,
    DateTime StartDate,
    DateTime EndDate
);
