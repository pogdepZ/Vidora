using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Dtos.Responses;

internal record UserStatusResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public UserData? Data { get; init; }
}
