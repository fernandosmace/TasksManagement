using Flunt.Validations;
using TasksManagement.Domain.Enums;

namespace TasksManagement.Domain.Entities;

public class TaskItem : Entity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? CompletionDate { get; private set; } // Novo campo para a data de conclusão
    public ETaskStatus Status { get; private set; }
    public ETaskPriority Priority { get; private set; }
    public Guid ProjectId { get; private set; }

    private readonly List<Comment> _comments = new();
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    private readonly List<TaskHistory> _history = new();
    public IReadOnlyCollection<TaskHistory> History => _history.AsReadOnly();

    public TaskItem(string title, string description, DateTime dueDate, ETaskPriority priority, Guid projectId)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        ProjectId = projectId;
        Status = ETaskStatus.Pending;

        Validar();
    }

    public void Update(string title, string description, DateTime dueDate, ETaskStatus status)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = status;

        if (status == ETaskStatus.Completed)
        {
            CompletionDate = DateTime.UtcNow; // Define a data de conclusão, se a tarefa é completada
        }
        else if (status == ETaskStatus.Pending)
        {
            CompletionDate = null; // Limpa a data de conclusão se a tarefa voltar a ser pendente
        }

        Validar();
    }

    protected override void Validar()
    {
        var contract = new Contract<TaskItem>()
            .Requires()
            .IsNotNullOrWhiteSpace(Title, nameof(Title), $"Campo {nameof(Title)} não foi informado.")
            .IsNotNullOrWhiteSpace(Description, nameof(Description), $"Campo {nameof(Description)} não foi informado.")
            .IsGreaterOrEqualsThan(DueDate, DateTime.UtcNow, nameof(DueDate), $"Campo {nameof(DueDate)} deve ser uma data futura.");

        if (Status == ETaskStatus.Completed && CompletionDate == null)
        {
            contract.AddNotification(nameof(CompletionDate), "Campo de conclusão deve estar preenchido quando a tarefa estiver completada.");
        }

        AddNotifications(contract);
    }
}