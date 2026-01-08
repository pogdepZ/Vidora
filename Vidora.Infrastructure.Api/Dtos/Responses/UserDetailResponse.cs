using System;
using System.Collections.Generic;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Dtos.Responses;

public record UserDetailResponse
{
    public bool Success { get; init; }
    public UserDetailData Data { get; init; } = new();
}

public record UserDetailData
{
    public UserData? User { get; init; }
    public List<SubscriptionData> Subscriptions { get; init; } = new();
    public List<OrderData> Orders { get; init; } = new();
}

