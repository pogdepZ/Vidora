using System;
using System.Collections.Generic;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

/// <summary>
/// DTO cho response chi ti?t user t? API
/// GET /api/users/{id}
/// </summary>
internal record UserDetailResponseDto
{
    public bool Success { get; init; }
    public UserDetailDataDto Data { get; init; } = new();
}

internal record UserDetailDataDto
{
    public UserInfoDto User { get; init; } = new();
    public List<SubscriptionDto> Subscriptions { get; init; } = new();
    public List<OrderDto> Orders { get; init; } = new();
}

internal record UserInfoDto
{
    public int UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Avatar { get; init; }
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string? Gender { get; init; }
    public DateTime? Birthday { get; init; }
}

internal record SubscriptionDto
{
    public int SubscriptionId { get; init; }
    public string PlanName { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string Status { get; init; } = string.Empty;
}

internal record OrderDto
{
    public int OrderId { get; init; }
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
