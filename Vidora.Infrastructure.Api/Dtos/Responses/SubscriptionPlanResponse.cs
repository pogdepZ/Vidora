using System.Collections.Generic;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Dtos.Responses;

public record SubscriptionPlansResponseDto
{
    public bool Success { get; init; }
    public IReadOnlyList<SubscriptionPlanData> Data { get; init; } = [];
}
