using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Models.ReportModels;

namespace TasksManagement.Domain.Interfaces.Repositories;
public interface ITaskRepository
{
    Task<Result<TaskItem?>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<TaskItem>>> GetByProjectIdAsync(Guid projectId);
    Task<Result> CreateAsync(TaskItem task);
    Task<Result> UpdateAsync(TaskItem task);
    Task<Result> DeleteAsync(TaskItem task);
    Task<Result<IEnumerable<TaskItem>>> GetCompletedTasksByProjectIdAsync(IEnumerable<Project> projects, DateTime dateThreshold);
    Task<Result<IEnumerable<TaskWithCommentCountReportModel>>> GetTopTasksWithMostCommentsAsync(DateTime dateThreshold);
}