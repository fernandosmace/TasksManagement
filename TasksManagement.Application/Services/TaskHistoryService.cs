using TasksManagement.Domain;
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

    public async Task<Result> CreateAsync(TaskHistory history)
    {
        try
        {
            await _taskHistoryRepository.CreateAsync(history);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("Falha ao gravar histórico da tarefa.", statusCode: 500);
        }
    }
}