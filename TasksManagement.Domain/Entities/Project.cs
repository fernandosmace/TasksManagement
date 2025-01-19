using Flunt.Validations;

namespace TasksManagement.Domain.Entities;
public class Project : Entity
{
    public string Name { get; private set; }
    public Guid UserId { get; private set; }

    private readonly List<Task> _tasks = [];
    public IReadOnlyCollection<Task> Tasks => _tasks.AsReadOnly();

    public Project(string name, Guid userId)
    {
        Name = name;
        UserId = userId;

        AddNotifications(new Contract<Project>()
            .Requires()
            .IsNotNullOrEmpty(Name, nameof(Name), $"Campo {nameof(Name)}não foi informado."));
    }

    public void AddTask(Task task)
    {
        if (_tasks.Count >= 20)
        {
            AddNotification(nameof(Tasks), "Não é possível inserir mais de 20 tarefas à um Projeto.");
            return;
        }

        _tasks.Add(task);
    }
    public void RemoveTask(Task task)
    {
        if (_tasks.Contains(task))
        {
            _tasks.Remove(task);
        }
        else
        {
            AddNotification(nameof(Tasks), "Tarefa não encontrada no projeto.");
        }
    }
}