using System.Collections.Generic;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;
using Vidora.Infrastructure.Api.Dtos.Responses.Metas;

namespace Vidora.Infrastructure.Api.Dtos.Responses;

public record UserPaginationResponse
{
    public bool Success { get; init; }
    public IReadOnlyList<UserData> Data { get; init; } = [];
    public PaginationMeta Pagination { get; init; } = new();
}
