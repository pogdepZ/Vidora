using System;
using System.Collections.Generic;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;
using Vidora.Infrastructure.Api.Dtos.Responses.Metas;

namespace Vidora.Infrastructure.Api.Dtos.Responses;

public record OrderPaginationResponseDto
{
    public bool Success { get; init; }
    public IReadOnlyList<OrderData>? Data { get; init; } = new List<OrderData>();
    public PaginationMeta Pagination { get; init; } = new();
}
