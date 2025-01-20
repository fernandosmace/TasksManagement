namespace TasksManagement.Domain.Models.OutputModels.Comments;
public record CommentWithTaskOutputModel
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
}
