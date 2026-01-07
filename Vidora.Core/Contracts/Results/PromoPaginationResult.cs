using System.Collections.Generic;

namespace Vidora.Core.Contracts.Results;

/// <summary>
/// Result cho danh sách Promos có pagination
/// GET /api/promos
/// </summary>
public record PromoPaginationResult(
    IReadOnlyList<PromoResult> Promos,
    PaginationResult Pagination
);
