using Flunt.Notifications;

public class Result<T> : Notifiable<Notification>
{
    public bool IsSuccess { get; private set; }
    public string? Message { get; private set; }
    public T? Data { get; private set; }
    public int? StatusCode { get; private set; }

    internal Result(bool isSuccess, string? message = null, T? data = default, int? statusCode = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
        StatusCode = statusCode;
    }
}

public class Result : Notifiable<Notification>
{
    public bool IsSuccess { get; private set; }
    public string? Message { get; private set; }
    public object? Data { get; private set; }
    public int? StatusCode { get; private set; }

    private Result(bool isSuccess, string? message = null, object? data = null, int? statusCode = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
        StatusCode = statusCode;
    }

    public static Result Success(string? message = null, object? data = null)
    {
        return new Result(true, message, data);
    }

    public static Result Failure(string? message = null, IEnumerable<Notification>? notifications = null, int? statusCode = null)
    {
        var result = new Result(false, message, statusCode: statusCode);

        if (notifications != null)
            result.AddNotifications((IReadOnlyCollection<Notification>)notifications.ToList());

        return result;
    }

    public static Result<T> Success<T>(T data, string? message = null)
    {
        return new Result<T>(true, message, data);
    }

    public static Result<T> Failure<T>(string? message = null, IEnumerable<Notification>? notifications = null, int? statusCode = null)
    {
        var result = new Result<T>(false, message, default, statusCode);

        if (notifications != null)
            result.AddNotifications((IReadOnlyCollection<Notification>)notifications.ToList());

        return result;
    }
}
