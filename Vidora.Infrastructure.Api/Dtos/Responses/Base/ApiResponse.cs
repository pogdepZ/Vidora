using System.Net;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Base;

public abstract record ApiResponse(
    HttpStatusCode StatusCode,
    string? Message = null)
{
    public abstract bool Success { get; }
}
