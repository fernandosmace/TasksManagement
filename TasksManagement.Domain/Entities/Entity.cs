using Flunt.Notifications;
using System.Diagnostics.CodeAnalysis;

namespace TasksManagement.Domain.Entities;

[ExcludeFromCodeCoverage]
public abstract class Entity : Notifiable<Notification>
{
    public Guid Id { get; private set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    protected abstract void Validar();
}