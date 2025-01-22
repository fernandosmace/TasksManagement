using MongoDB.Driver;
using TasksManagement.Domain.Entities;

namespace TasksManagement.Infrastructure.Database;
public interface IMongoDbContext
{
    IMongoCollection<TaskHistory> TaskHistories { get; }
}