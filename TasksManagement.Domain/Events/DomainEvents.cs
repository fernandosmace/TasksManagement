using MediatR;

namespace TasksManagement.Domain.Events;
public static class DomainEvents
{
    private static readonly List<INotificationHandler<TaskUpdatedEvent>> Handlers = [];

    public static void RegisterHandler(INotificationHandler<TaskUpdatedEvent> handler)
    {
        Handlers.Add(handler);
    }

    public static void Raise(TaskUpdatedEvent @event)
    {
        foreach (var handler in Handlers)
        {
            handler.Handle(@event, new CancellationToken()).Wait();
        }
    }
}