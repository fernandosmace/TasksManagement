using System.Diagnostics.CodeAnalysis;

namespace TasksManagement.Domain.Models.ReportModels;

[ExcludeFromCodeCoverage]
public class ProjectWithTasksCountReportModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int TasksCount { get; set; }
}