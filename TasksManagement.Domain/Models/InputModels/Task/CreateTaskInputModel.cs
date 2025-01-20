using System.ComponentModel.DataAnnotations;
using TasksManagement.Domain.Enums;

namespace TasksManagement.API.Models.InputModels.Task;

public record CreateTaskInputModel
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public ETaskPriority Priority { get; set; }

    [Required(ErrorMessage = "Id do Projeto não informado.")]
    public Guid ProjectId { get; private set; }

    public CreateTaskInputModel(string? title, string? description, DateTime dueDate, ETaskPriority priority, Guid projectId)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        ProjectId = projectId;
    }
}
