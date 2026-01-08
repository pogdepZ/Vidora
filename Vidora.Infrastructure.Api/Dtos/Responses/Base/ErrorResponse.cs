using System.Net;

namespace Vidora.Infrastructure.Api.Dtos.Responses.Base;

public record ErrorResponse(
    object Error,
    HttpStatusCode StatusCode,
    string? Message = null
) : ApiResponse(StatusCode, Message)
{
    public override bool Success => false;
}