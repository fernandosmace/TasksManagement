using TasksManagement.API.Models.OutputModels.Task;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Interfaces.Services;

namespace TasksManagement.Application.Services;
public class TaskReportService : ITaskReportService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;

    public TaskReportService(IProjectRepository projectRepository, ITaskRepository taskRepository)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
    }

    public async Task<Result<IEnumerable<TaskReportModel>>> GenerateCompletedTasksByUserReportAsync(Guid userId, int days = 30)
    {
        var projects = await _projectRepository.GetAllByUserIdAsync(userId);

        if (projects == null)
            return Result.Success(Enumerable.Empty<TaskReportModel>());

        var dateThreshold = DateTime.UtcNow.AddDays(-days);
        var tasks = await _taskRepository.GetCompletedTasksByProjectIdAsync(projects, dateThreshold);

        var reports = tasks.Select(task => new TaskReportModel
        {
            TaskId = task.Id,
            Title = task.Title,
            CompletionDate = task.CompletionDate!.Value
        });

        return Result.Success<IEnumerable<TaskReportModel>>(reports.ToList());
    }

    public async Task<Result<IEnumerable<TaskReportModel>>> GenerateTopTasksByCommentsAsync(int days = 30)
    {
        var dateThreshold = DateTime.UtcNow.AddDays(-days);
        var tasksWithComments = await _taskRepository.GetTopTasksWithMostCommentsAsync(dateThreshold);

        var reports = tasksWithComments.Select(x => new TaskReportModel
        {
            TaskId = x.TaskId,
            Title = x.Title,
            CommentCount = x.CommentCount
        });

        return Result.Success<IEnumerable<TaskReportModel>>(reports.ToList());
    }
}