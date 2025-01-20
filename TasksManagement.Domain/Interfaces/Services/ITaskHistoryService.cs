using TasksManagement.Domain.Entities;

namespace TasksManagement.Domain.Interfaces.Services;
public interface ITaskHistoryService
{
    Task CreateAsync(TaskHistory history);
}