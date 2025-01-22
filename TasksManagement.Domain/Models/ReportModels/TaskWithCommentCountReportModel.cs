using System.Diagnostics.CodeAnalysis;

namespace TasksManagement.Domain.Models.ReportModels;

[ExcludeFromCodeCoverage]
public class TaskWithCommentCountReportModel
{
    public Guid TaskId { get; set; }
    public string Title { get; set; }
    public int CommentCount { get; set; }
}