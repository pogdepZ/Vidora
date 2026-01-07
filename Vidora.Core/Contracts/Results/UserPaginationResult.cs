using System.Collections.Generic;

namespace Vidora.Core.Contracts.Results;

/// <summary>
/// Result cho danh sách users có pagination
/// </summary>
public record UserPaginationResult(
    IReadOnlyList<AdminUserResult> Users,
    PaginationResult Pagination
);
