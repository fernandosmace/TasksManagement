namespace TasksManagement.Domain.Models.ReportModels;
public class TaskWithCommentCountReportModel
{
    public Guid TaskId { get; set; }
    public string Title { get; set; }
    public int CommentCount { get; set; }
}