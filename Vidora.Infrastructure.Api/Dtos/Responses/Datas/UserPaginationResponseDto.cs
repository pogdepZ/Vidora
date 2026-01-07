using System;
using System.Collections.Generic;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

/// <summary>
/// DTO cho response danh sách users có pagination t? API
/// GET /api/users
/// </summary>
internal record UserPaginationResponseDto
{
    public bool Success { get; init; }
    public List<UserItemDto> Data { get; init; } = new();
    public PaginationDto Pagination { get; init; } = new();
}

/// <summary>
/// DTO cho t?ng user item trong danh sách
/// </summary>
internal record UserItemDto
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

/// <summary>
/// DTO cho pagination info
/// </summary>
internal record PaginationDto
{
    public int Page { get; init; }
    public int Limit { get; init; }
    public int Total { get; init; }
    public int TotalPages { get; init; }
}
