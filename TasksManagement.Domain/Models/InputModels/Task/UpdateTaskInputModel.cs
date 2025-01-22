using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TasksManagement.API.Models.InputModels.User;
using TasksManagement.Domain.Enums;

namespace TasksManagement.API.Models.InputModels.Task;

[ExcludeFromCodeCoverage]
public record UpdateTaskInputModel
{
    [Required(ErrorMessage = "Usuário não informado")]
    public UserInputModel User { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public ETaskStatus Status { get; set; }

    public UpdateTaskInputModel(UserInputModel user, string? title, string? description, DateTime dueDate, ETaskStatus status)
    {
        User = user;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = status;
    }
}
