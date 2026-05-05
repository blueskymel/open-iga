namespace OpenIga.Api.Services;

public enum ServiceError
{
    NotFound,
    Conflict,
    Validation
}

public class ServiceResult<T>
{
    private ServiceResult(T? value, ServiceError? error, string? message)
    {
        Value = value;
        Error = error;
        Message = message;
    }

    public T? Value { get; }
    public ServiceError? Error { get; }
    public string? Message { get; }
    public bool Succeeded => Error is null;

    public static ServiceResult<T> Success(T value) => new(value, null, null);

    public static ServiceResult<T> Failure(ServiceError error, string message) => new(default, error, message);
}

public class ServiceResult
{
    private ServiceResult(ServiceError? error, string? message)
    {
        Error = error;
        Message = message;
    }

    public ServiceError? Error { get; }
    public string? Message { get; }
    public bool Succeeded => Error is null;

    public static ServiceResult Success() => new(null, null);

    public static ServiceResult Failure(ServiceError error, string message) => new(error, message);
}
