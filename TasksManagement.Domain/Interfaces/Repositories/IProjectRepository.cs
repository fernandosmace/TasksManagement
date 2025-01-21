using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Models.ReportModels;

namespace TasksManagement.Domain.Interfaces.Repositories;
public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id);
    Task<IEnumerable<Project>> GetAllByUserIdAsync(Guid userId);
    Task CreateAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Project project);
    Task<IEnumerable<ProjectWithTasksCountReportModel>> GetTopProjectsWithMostCompletedTasksAsync(int days);
}