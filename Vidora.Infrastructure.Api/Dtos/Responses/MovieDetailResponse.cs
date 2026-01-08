using System.Collections.Generic;
using System.Text.Json.Serialization;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Dtos.Responses;

internal class MovieDetailResponse
{
    public bool Success { get; set; }

    public required MovieData Data { get; set; }
}

