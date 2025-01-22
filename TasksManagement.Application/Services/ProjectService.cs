using TasksManagement.API.Models.InputModels.Project;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Interfaces.Services;

namespace TasksManagement.Application.Services;
public class ProjectService : IProjectService
{
    private readonly IUserService _userService;
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;

    public ProjectService(IUserService userService, IProjectRepository projectRepository, ITaskRepository taskRepository)
    {
        _userService = userService;
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
    }

    public async Task<Result<Project>> GetByIdAsync(Guid id)
    {
        try
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (!project.IsValid)
                return Result.Failure<Project>("Projeto não encontrado.", statusCode: 404);

            return Result.Success(project.Data!);
        }
        catch (Exception ex)
        {
            return Result.Failure<Project>("Ocorreu um erro inesperado", statusCode: 500);
        }
    }

    public async Task<Result<IEnumerable<Project>>> GetAllByUserIdAsync(Guid userId)
    {
        try
        {
            var projects = await _projectRepository.GetAllByUserIdAsync(userId);
            if (!projects.IsValid)
                return Result.Failure<IEnumerable<Project>>("Nenhum projeto encontrado para o usuário.", statusCode: 404);

            return Result.Success(projects.Data!);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<Project>>("Ocorreu um erro inesperado", statusCode: 500);
        }
    }

    public async Task<Result<IEnumerable<TaskItem>>> GetTasksByProjectIdAsync(Guid projectId)
    {
        try
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (!project.IsValid)
                return Result.Failure<IEnumerable<TaskItem>>("Projeto não encontrado.", statusCode: 404);

            var tasks = await _taskRepository.GetByProjectIdAsync(projectId);
            if (!tasks.IsValid)
                return Result.Failure<IEnumerable<TaskItem>>("Falha ao obter tarefa.", statusCode: 500);

            return Result.Success(tasks.Data!);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<TaskItem>>("Ocorreu um erro inesperado", statusCode: 500);
        }
    }

    public async Task<Result<Project>> CreateAsync(CreateProjectInputModel inputModel)
    {
        try
        {
            var userResult = await _userService.GetByIdAsync(inputModel.User!.Id);
            var user = userResult.Data;

            if (!userResult.IsValid || user == null)
            {
                // Se o usuário não existir, cria um novo
                user = new User(inputModel.User!.Name!, inputModel.User!.Role!);
                var userCreated = await _userService.CreateAsync(user);

                if (!userCreated.IsValid)
                {
                    var userErrors = string.Join(", ", userCreated.Notifications.Select(n => n.Message));
                    return Result.Failure<Project>(
                        $"Erro ao criar o usuário: {userErrors}",
                        statusCode: 422
                    );
                }
            }

            // Verificar se o usuário é válido
            if (!user.IsValid)
            {
                var userErrors = string.Join(", ", user.Notifications.Select(n => n.Message));
                return Result.Failure<Project>(
                    $"Erro ao validar o usuário: {userErrors}",
                    statusCode: 422
                );
            }

            var project = new Project(inputModel.Name!, user.Id);

            // Verificar se o projeto é válido
            if (!project.IsValid)
            {
                var projectErrors = string.Join(", ", project.Notifications.Select(n => n.Message));
                return Result.Failure<Project>(
                    $"Erro ao validar o projeto: {projectErrors}",
                    statusCode: 422
                );
            }

            var createdProject = await _projectRepository.CreateAsync(project);
            if (!createdProject.IsValid)
                return Result.Failure<Project>($"Erro ao criar o projeto.", statusCode: 422);

            return Result.Success(project);
        }
        catch (Exception ex)
        {
            return Result.Failure<Project>("Ocorreu um erro inesperado", statusCode: 500);
        }
    }


    public async Task<Result> UpdateAsync(Guid id, UpdateProjectInputModel inputModel)
    {
        try
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (!project.IsValid)
                return Result.Failure("Projeto não encontrado.", statusCode: 404);

            project.Data!.Update(inputModel.Name!);
            if (!project.IsValid)
            {
                var errors = string.Join(", ", project.Notifications.Select(n => n.Message));
                return Result.Failure($"Erro ao atualizar o projeto: {errors}", statusCode: 422);
            }

            var udpatedProject = await _projectRepository.UpdateAsync(project.Data);
            if (!udpatedProject.IsValid)
                return Result.Failure($"Erro ao atualizar o projeto", statusCode: 500);

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
            var project = await _projectRepository.GetByIdAsync(id);
            if (!project.IsValid)
                return Result.Failure("Projeto não encontrado.", statusCode: 404);

            project.Data!.ValidateForDelete();
            if (!project.IsValid)
            {
                var errors = string.Join(", ", project.Notifications.Select(n => n.Message));
                return Result.Failure($"Erro ao validar a exclusão do projeto: {errors}", statusCode: 422);
            }

            var deletedProject = await _projectRepository.DeleteAsync(project.Data);
            if (!deletedProject.IsValid)
                return Result.Failure($"Erro ao durante a exclusão do projeto", statusCode: 500);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("Ocorreu um erro inesperado", statusCode: 500);
        }
    }
}