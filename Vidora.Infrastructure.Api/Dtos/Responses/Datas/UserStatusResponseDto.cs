namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

/// <summary>
/// DTO cho response toggle status user t? API
/// PUT /api/users/{id}/status
/// </summary>
internal record UserStatusResponseDto
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public UserStatusDataDto? Data { get; init; }
}

internal record UserStatusDataDto
{
    public int UserId { get; init; }
    public string Status { get; init; } = string.Empty;
}
