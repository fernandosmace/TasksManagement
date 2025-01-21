namespace TasksManagement.Domain.Models.OutputModels.Project;
public class ProjectReportModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int CompletedTaskCount { get; set; }
}