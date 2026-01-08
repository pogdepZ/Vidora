using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Vidora.Core.Entities;

namespace Vidora.Core.Contracts.Results
{
    public record MoviePaginationResult(
    List<AdminMovie> Movies,
    PaginationResult Pagination
);
}
