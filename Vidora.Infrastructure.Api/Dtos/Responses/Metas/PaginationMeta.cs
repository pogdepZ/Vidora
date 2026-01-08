namespace Vidora.Infrastructure.Api.Dtos.Responses.Metas;

public record PaginationMeta(
    int Page = 1,
    int Limit = 10,
    int Total = 0,
    int TotalPages = 0
);
