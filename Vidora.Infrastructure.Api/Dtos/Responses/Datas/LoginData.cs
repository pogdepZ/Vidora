namespace Vidora.Infrastructure.Api.Dtos.Responses.Datas;

public record LoginData(
    UserData User,
    string AccessToken,
    string ExpiresIn
);
