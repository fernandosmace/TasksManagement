using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Infrastructure.Database;

namespace TasksManagement.Infrastructure.Repositories;
public class TaskHistoryRepository : ITaskHistoryRepository
{
    private readonly MongoDbContext _mongoDbContext;

    public TaskHistoryRepository(MongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    public async Task CreateAsync(TaskHistory history) => await _mongoDbContext.TaskHistories.InsertOneAsync(history);
}