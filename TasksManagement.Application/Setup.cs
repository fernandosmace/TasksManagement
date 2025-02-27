﻿using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TasksManagement.Application.Services;
using TasksManagement.Domain.Interfaces.Services;
using TasksManagement.Services;

namespace TasksManagement.Application;


[ExcludeFromCodeCoverage]
public static class Setup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        AddServices(services);

        return services;
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ITaskHistoryService, TaskHistoryService>();
        services.AddScoped<ITaskReportService, TaskReportService>();
        services.AddScoped<IProjectReportService, ProjectReportService>();
    }
}