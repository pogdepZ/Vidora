namespace Vidora.Infrastructure.Api.Dtos.Requests;

public record RegisterRequest(
    string Username,
    string FullName,
    string Email,
    string Password
    );
