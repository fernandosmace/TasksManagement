using Flunt.Notifications;

namespace TasksManagement.Domain.Models.OutputModels;
public record ErrorResultOutputModel
{
    public string? Message { get; init; }
    public IList<Notification> Errors { get; init; }

    public ErrorResultOutputModel(string? message, IList<Notification> errors)
    {
        Message = message;
        Errors = errors ?? [];
    }
}
