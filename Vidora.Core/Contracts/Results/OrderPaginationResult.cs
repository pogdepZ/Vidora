using System.Collections.Generic;

namespace Vidora.Core.Contracts.Results;

/// <summary>
/// Result cho danh sách Orders có pagination
/// GET /api/orders/all
/// </summary>
public record OrderPaginationResult(
    IReadOnlyList<OrderResult> Orders,
    PaginationResult Pagination
);
