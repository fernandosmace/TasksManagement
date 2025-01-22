using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Models.ReportModels;

namespace TasksManagement.Domain.Interfaces.Repositories;
public interface IProjectRepository
{
    Task<Result<Project>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<Project>>> GetAllByUserIdAsync(Guid userId);
    Task<Result> CreateAsync(Project project);
    Task<Result> UpdateAsync(Project project);
    Task<Result> DeleteAsync(Project project);
    Task<Result<IEnumerable<ProjectWithTasksCountReportModel>>> GetTopProjectsWithMostCompletedTasksAsync(int days);
}