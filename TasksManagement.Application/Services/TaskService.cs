﻿using TasksManagement.API.Models.InputModels.Task;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Enums;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Interfaces.Services;

namespace TasksManagement.Services;
public class TaskService : ITaskService
{
    private readonly IProjectService _projectService;
    private readonly IUserService _userService;
    private readonly ITaskRepository _taskRepository;

    public TaskService(IProjectService projectService, IUserService userService, ITaskRepository taskRepository)
    {
        _projectService = projectService;
        _userService = userService;
        _taskRepository = taskRepository;
    }

    public async Task<Result<TaskItem>> GetByIdAsync(Guid id)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (!task.IsValid)
                return Result.Failure<TaskItem>("Tarefa não encontrada.", statusCode: 404);

            return Result.Success(task.Data);
        }
        catch (Exception ex)
        {
            return Result.Failure<TaskItem>("Ocorreu um erro inesperado.", statusCode: 500);
        }
    }

    public async Task<Result<TaskItem>> CreateAsync(CreateTaskInputModel inputModel)
    {
        try
        {
            var project = await _projectService.GetByIdAsync(inputModel.ProjectId);
            if (project == null || project.Data == null)
                return Result.Failure<TaskItem>("Projeto não encontrado.", statusCode: 404);

            var pendingTasks = project.Data.Tasks.Where(t => t.Status == ETaskStatus.Pending).ToList();
            if (pendingTasks.Count >= 20)
                return Result.Failure<TaskItem>("Não é possível adicionar mais de 20 tarefas por projeto. Finalize ou remova tarefas existentes para adicionar uma nova tarefa.", statusCode: 422);

            var task = new TaskItem(inputModel.Title!, inputModel.Description!, inputModel.DueDate, inputModel.Priority, inputModel.ProjectId);

            if (!task.IsValid)
            {
                var taskErrors = string.Join(", ", task.Notifications.Select(n => n.Message));
                return Result.Failure<TaskItem>(
                    $"Erro ao validar a Task: {taskErrors}",
                    statusCode: 422
                );
            }

            var createdTask = await _taskRepository.CreateAsync(task);
            if (!createdTask.IsValid)
                return Result.Failure<TaskItem>("Erro ao criar a tarefa.", statusCode: 500);

            return Result.Success(task);
        }
        catch (Exception ex)
        {
            return Result.Failure<TaskItem>("Ocorreu um erro inesperado", statusCode: 500);
        }
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateTaskInputModel inputModel)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (!task.IsValid)
                return Result.Failure(message: "Tarefa não encontrado.", statusCode: 404);

            var user = await _userService.GetByIdAsync(inputModel.User!.Id);
            if (!user.IsValid)
                return Result.Failure(message: "Usuário não encontrado.", statusCode: 404);

            task.Data!.Update(inputModel.Title!, inputModel.Description!, inputModel.DueDate, inputModel.Status);
            if (!task.IsValid)
            {
                var taskErrors = string.Join(", ", task.Notifications.Select(n => n.Message));
                return Result.Failure(
                    $"Erro ao validar a Task: {taskErrors}",
                    statusCode: 422
                );
            }

            var updatedTask = await _taskRepository.UpdateAsync(task.Data);
            if (!updatedTask.IsValid)
                return Result.Failure(message: "Erro ao atualizar a tarefa.", statusCode: 500);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("Ocorreu um erro inesperado", statusCode: 500);
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (!task.IsValid)
                return Result.Failure("Tarefa não encontrada.", statusCode: 404);

            var deletedTask = await _taskRepository.DeleteAsync(task.Data!);
            if (!deletedTask.IsValid)
                return Result.Failure("Erro ao deletar  a tarefa.", statusCode: 500);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("Ocorreu um erro inesperado", statusCode: 500);
        }
    }
}