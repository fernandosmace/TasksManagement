using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Interfaces.Services;
using TasksManagement.Domain.Models.OutputModels.Project;

namespace TasksManagement.Application.Services;
public class ProjectReportService : IProjectReportService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;

    public ProjectReportService(IProjectRepository projectRepository, ITaskRepository taskRepository)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
    }

    public async Task<Result<IEnumerable<ProjectReportModel>>> GenerateTopProjectsByCompletedTasksAsync(int days = 30)
    {
        var projects = await _projectRepository.GetTopProjectsWithMostCompletedTasksAsync(days);

        if (projects == null || !projects.Any())
            return Result.Success(Enumerable.Empty<ProjectReportModel>());

        var projectReport = projects.Select(p => new ProjectReportModel
        {
            Id = p.Id,
            Name = p.Name,
            CompletedTaskCount = p.TasksCount
        }).ToList();

        return Result.Success<IEnumerable<ProjectReportModel>>(projectReport);
    }
}