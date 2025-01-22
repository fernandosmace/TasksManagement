using System.Diagnostics.CodeAnalysis;

namespace TasksManagement.Domain.Models.OutputModels.Comments;


[ExcludeFromCodeCoverage]
public record CommentWithTaskOutputModel
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
}
