using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Models.ReportModels;

namespace TasksManagement.Domain.Interfaces.Repositories;
public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId);
    Task CreateAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);

    Task<IEnumerable<TaskItem>> GetCompletedTasksByProjectIdAsync(IEnumerable<Project> projects, DateTime dateThreshold);

    Task<IEnumerable<TaskWithCommentCountReportModel>> GetTopTasksWithMostCommentsAsync(DateTime dateThreshold);
}