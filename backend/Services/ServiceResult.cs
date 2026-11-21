namespace backend.Services;

// Результат операции сервиса: успех с данными или понятный отказ
// с кодом 400/403/404/409, который контроллер отдаёт как ProblemDetails.
public sealed class ServiceResult<T>
{
    public bool Success { get; private init; }

    public T? Value { get; private init; }

    public int? StatusCode { get; private init; }

    public string? Error { get; private init; }

    public static ServiceResult<T> Ok(T value) => new() { Success = true, Value = value };

    public static ServiceResult<T> Fail(int statusCode, string message) =>
        new() { Success = false, StatusCode = statusCode, Error = message };
}
