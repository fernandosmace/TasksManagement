using MongoDB.Driver;
using TasksManagement.Domain.Entities;

namespace TasksManagement.Infrastructure.Database;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<TaskHistory> TaskHistories => _database.GetCollection<TaskHistory>("TaskHistory");
}
