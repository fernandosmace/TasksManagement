using Flunt.Notifications;

namespace TasksManagement.Domain.Entities;
public abstract class Entity : Notifiable<Notification>
{
    public Guid Id { get; private set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    protected abstract void Validar();
}