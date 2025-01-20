using Flunt.Validations;
using TasksManagement.Domain.Enums;

namespace TasksManagement.Domain.Entities;
public class Project : Entity
{
    public string Name { get; private set; }
    public Guid UserId { get; private set; }

    private readonly List<TaskItem> _tasks = [];
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    public Project(string name, Guid userId)
    {
        Name = name;
        UserId = userId;

        Validar();
    }

    public void Update(string name)
    {
        Name = name;

        Validar();
    }

    protected override void Validar()
    {
        AddNotifications(new Contract<Project>()
            .Requires()
            .IsNotNullOrEmpty(Name, nameof(Name), $"Campo {nameof(Name)} não foi informado."));
    }

    public void ValidateForDelete()
    {
        if (_tasks.Any(t => t.Status == ETaskStatus.Pending))
        {
            AddNotification(nameof(Tasks), "Não é possível remover o projeto enquanto houver tarefas pendentes. Conclua ou remova essas tarefas antes de excluir o projeto.");
        }
    }
}