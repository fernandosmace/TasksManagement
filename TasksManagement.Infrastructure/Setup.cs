using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Infrastructure.Database;
using TasksManagement.Infrastructure.Repositories;

namespace TasksManagement.Infrastructure;

[ExcludeFromCodeCoverage]
public static class Setup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDatabases(services, configuration);
        AddRepositories(services);

        return services;
    }

    private static void AddDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        AddSqlDb(services, configuration);
        AddMongoDb(services, configuration);
    }

    private static void AddSqlDb(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

        var server = Environment.GetEnvironmentVariable("SQL_HOST") ?? string.Empty;
        var port = Environment.GetEnvironmentVariable("SQL_PORT") ?? string.Empty;
        var user = Environment.GetEnvironmentVariable("SQL_USER") ?? string.Empty;
        var password = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? string.Empty;

        sqlConnectionString = sqlConnectionString
            .Replace("{HOST}", server)
            .Replace("{PORT}", port)
            .Replace("{USER}", user)
            .Replace("{PASSWORD}", password);

        services.AddDbContext<SqlDbContext>(options =>
            options.UseSqlServer(sqlConnectionString));
    }

    private static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMongoDbContext>(sp =>
        {
            var mongoConnectionString = configuration["MongoDb:ConnectionString"] ?? string.Empty;
            var databaseName = configuration["MongoDb:DatabaseName"] ?? string.Empty;

            var server = Environment.GetEnvironmentVariable("MONGO_HOST") ?? string.Empty;
            var port = Environment.GetEnvironmentVariable("MONGO_PORT") ?? string.Empty;
            var user = Environment.GetEnvironmentVariable("MONGO_USER") ?? string.Empty;
            var password = Environment.GetEnvironmentVariable("MONGO_PASSWORD") ?? string.Empty;

            mongoConnectionString = mongoConnectionString
                .Replace("{HOST}", server)
                .Replace("{PORT}", port)
                .Replace("{USER}", user)
                .Replace("{PASSWORD}", password);

            if (string.IsNullOrEmpty(mongoConnectionString))
                throw new ArgumentNullException("MongoDb:ConnectionString", "A connection string do MongoDB não pode ser nula ou vazia.");

            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentNullException("MongoDb:DatabaseName", "O nome do banco de dados do MongoDB não pode ser nulo ou vazio.");

            return new MongoDbContext(mongoConnectionString, databaseName);
        });
    }


    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ITaskHistoryRepository, TaskHistoryRepository>();
    }
}