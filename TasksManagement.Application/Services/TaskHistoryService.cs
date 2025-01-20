using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Interfaces.Services;

namespace TasksManagement.Application.Services;
public class TaskHistoryService : ITaskHistoryService
{
    private readonly ITaskHistoryRepository _taskHistoryRepository;

    public TaskHistoryService(ITaskHistoryRepository taskHistoryRepository)
    {
        _taskHistoryRepository = taskHistoryRepository;
    }

    public async Task CreateAsync(TaskHistory history) => await _taskHistoryRepository.CreateAsync(history);
}