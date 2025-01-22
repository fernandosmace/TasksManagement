using Flunt.Notifications;

namespace TasksManagement.Domain;
public class Result<T> : Notifiable<Notification>
{
    public string? Message { get; private set; }
    public T? Data { get; private set; }
    public int? StatusCode { get; private set; }

    internal Result(string? message = null, T? data = default, int? statusCode = null)
    {
        Message = message;
        Data = data;
        StatusCode = statusCode;
    }
}

public class Result : Notifiable<Notification>
{
    public string? Message { get; private set; }
    public object? Data { get; private set; }
    public int? StatusCode { get; private set; }

    private Result(string? message = null, object? data = null, int? statusCode = null)
    {
        Message = message;
        Data = data;
        StatusCode = statusCode;
    }

    public static Result Success(string? message = null, object? data = null)
    {
        return new Result(message, data);
    }

    public static Result Failure(string? message = null, IEnumerable<Notification>? notifications = null, int? statusCode = null)
    {
        var result = new Result(message, statusCode: statusCode);

        // Se não houver notificações fornecidas, adicione uma notificação genérica de falha
        if (notifications == null || !notifications.Any())
            result.AddNotification("GenericFailure", "A operação falhou.");
        else
            result.AddNotifications((IReadOnlyCollection<Notification>)notifications.ToList());

        return result;
    }

    public static Result<T> Success<T>(T data, string? message = null)
    {
        return new Result<T>(message, data);
    }

    public static Result<T> Failure<T>(string? message = null, IEnumerable<Notification>? notifications = null, int? statusCode = null)
    {
        var result = new Result<T>(message, default, statusCode);

        if (notifications == null || !notifications.Any())
            result.AddNotification("GenericFailure", "A operação falhou.");
        else
            result.AddNotifications((IReadOnlyCollection<Notification>)notifications.ToList());

        return result;
    }
}
