using TasksManagement.API.Models.InputModels.Project;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Interfaces.Services;

namespace TasksManagement.Application.Services
{
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
                if (project == null)
                    return Result.Failure<Project>("Projeto não encontrado.", statusCode: 404);

                return Result.Success(project);
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
                if (projects == null || !projects.Any())
                    return Result.Failure<IEnumerable<Project>>("Nenhum projeto encontrado para o usuário.", statusCode: 404);

                return Result.Success(projects);
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
                if (project == null)
                    return Result.Failure<IEnumerable<TaskItem>>("Projeto não encontrado.", statusCode: 404);

                var tasks = await _taskRepository.GetByProjectIdAsync(projectId);
                return Result.Success(tasks);
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
                if (user == null)
                {
                    user = new User(inputModel.User!.Name!, inputModel.User!.Role!);
                    var userCreated = await _userService.CreateAsync(user);

                    if (!userCreated.IsSuccess)
                    {
                        var userErrors = string.Join(", ", userCreated.Notifications.Select(n => n.Message));
                        return Result.Failure<Project>(
                            $"Erro ao criar o usuário: {userErrors}",
                            statusCode: 422
                        );
                    }
                }

                if (!user.IsValid)
                {
                    var userErrors = string.Join(", ", user.Notifications.Select(n => n.Message));
                    return Result.Failure<Project>(
                        $"Erro ao validar o usuário: {userErrors}",
                        statusCode: 422
                    );
                }

                var project = new Project(inputModel.Name!, user.Id);
                if (!project.IsValid)
                {
                    var projectErrors = string.Join(", ", project.Notifications.Select(n => n.Message));
                    return Result.Failure<Project>(
                        $"Erro ao validar o projeto: {projectErrors}",
                        statusCode: 422
                    );
                }

                await _projectRepository.CreateAsync(project);

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
                if (project == null)
                    return Result.Failure("Projeto não encontrado.", statusCode: 404);

                project.Update(inputModel.Name!);
                if (!project.IsValid)
                {
                    var errors = string.Join(", ", project.Notifications.Select(n => n.Message));
                    return Result.Failure($"Erro ao atualizar o projeto: {errors}", statusCode: 422);
                }

                await _projectRepository.UpdateAsync(project);

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
                if (project == null)
                    return Result.Failure("Projeto não encontrado.", statusCode: 404);

                project.ValidateForDelete();
                if (!project.IsValid)
                {
                    var errors = string.Join(", ", project.Notifications.Select(n => n.Message));
                    return Result.Failure($"Erro ao validar a exclusão do projeto: {errors}", statusCode: 422);
                }

                await _projectRepository.DeleteAsync(project);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure("Ocorreu um erro inesperado", statusCode: 500);
            }
        }
    }
}
