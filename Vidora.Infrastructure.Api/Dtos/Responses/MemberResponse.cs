using System.Collections.Generic;
using System.Text.Json.Serialization;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Dtos.Responses;

public record MembersResponseDto
{
    public bool Success { get; set; }
    public IReadOnlyList<MemberData> Data { get; set; } = [];
}
