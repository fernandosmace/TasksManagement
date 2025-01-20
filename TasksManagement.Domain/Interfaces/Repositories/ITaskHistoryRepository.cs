using TasksManagement.Domain.Entities;

namespace TasksManagement.Domain.Interfaces.Repositories;
public interface ITaskHistoryRepository
{
    Task CreateAsync(TaskHistory history);
}