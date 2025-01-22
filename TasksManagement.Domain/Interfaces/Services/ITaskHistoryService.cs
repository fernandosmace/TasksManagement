using TasksManagement.Domain.Entities;

namespace TasksManagement.Domain.Interfaces.Services;
public interface ITaskHistoryService
{
    Task<Result> CreateAsync(TaskHistory history);
}