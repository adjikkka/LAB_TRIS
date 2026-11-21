namespace backend.Dtos;

// Ответ POST /api/auth/login: учебный JWT + данные вошедшего пользователя.
public sealed class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; set; }

    public LoginUserResponse User { get; set; } = new();
}

public sealed class LoginUserResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
