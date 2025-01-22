using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Infrastructure.Database;

namespace TasksManagement.Infrastructure.Repositories;
public class TaskHistoryRepository : ITaskHistoryRepository
{
    private readonly IMongoDbContext _mongoDbContext;

    public TaskHistoryRepository(IMongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task<Result> CreateAsync(TaskHistory history)
    {
        try
        {
            await _mongoDbContext.TaskHistories.InsertOneAsync(history);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("Falha ao gravar histórico da tarefa.");
        }
    }
}