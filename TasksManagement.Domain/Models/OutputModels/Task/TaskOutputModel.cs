using System.Diagnostics.CodeAnalysis;
using TasksManagement.Domain.Enums;
using TasksManagement.Domain.Models.OutputModels.Comments;

namespace TasksManagement.Domain.Models.OutputModels.Task;

[ExcludeFromCodeCoverage]
public record TaskOutputModel
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CompletionDate { get; set; }
    public ETaskStatus Status { get; set; }
    public ETaskPriority Priority { get; set; }
    public IEnumerable<CommentOutputModel> Comments { get; set; } = [];
}