namespace Vidora.Infrastructure.Api.Dtos.Requests;

public record LoginRequest(
    string Email,
    string Password
);