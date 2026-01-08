using System;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

public record OrderData(
    int OrderId,
    decimal Amount,
    string PaymentMethodd,
    string Status,
    string FullName,
    string Email,
    DateTime CreatedAt
    );
