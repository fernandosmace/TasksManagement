using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TasksManagement.API.Models.InputModels.User;

namespace TasksManagement.API.Models.InputModels.Comment;

[ExcludeFromCodeCoverage]
public record CreateCommentInputModel
{
    public Guid TaskId { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }

    [Required(ErrorMessage = "Usuário não informado")]
    public UserInputModel User { get; set; }

    public CreateCommentInputModel(Guid taskId, string? content, DateTime createdAt, UserInputModel user)
    {
        TaskId = taskId;
        Content = content;
        CreatedAt = createdAt;
        User = user;
    }
}