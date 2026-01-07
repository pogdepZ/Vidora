using System.Collections.Generic;
using System.Text.Json.Serialization;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Dtos.Responses;

internal record GenreResponse
{
    public bool Success { get; set; }

    public IReadOnlyList<GenreData> Data { get; init; } = [];
}
