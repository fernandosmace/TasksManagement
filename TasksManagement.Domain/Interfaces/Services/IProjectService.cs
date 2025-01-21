using TasksManagement.API.Models.InputModels.Project;
using TasksManagement.Domain.Entities;

namespace TasksManagement.Domain.Interfaces.Services;

public interface IProjectService
{
    /// <summary>
    /// Obtém um projeto pelo ID.
    /// </summary>
    /// <param name="id">O ID do projeto.</param>
    /// <returns>O resultado contendo o projeto ou erro.</returns>
    Task<Result<Project>> GetByIdAsync(Guid id);

    /// <summary>
    /// Obtém todos os projetos de um usuário.
    /// </summary>
    /// <param name="userId">O ID do usuário.</param>
    /// <returns>O resultado contendo os projetos ou erro.</returns>
    Task<Result<IEnumerable<Project>>> GetAllByUserIdAsync(Guid userId);

    /// <summary>
    /// Obtém as tarefas associadas a um projeto específico.
    /// </summary>
    /// <param name="projectId">O ID do projeto cujas tarefas serão recuperadas.</param>
    /// <returns>O resultado contendo uma lista de tarefas associadas ao projeto ou um erro, caso o projeto não exista ou ocorram problemas na busca das tarefas.</returns>
    Task<Result<IEnumerable<TaskItem>>> GetTasksByProjectIdAsync(Guid projectId);

    /// <summary>
    /// Cria um novo projeto.
    /// </summary>
    /// <param name="inputModel">Os dados de entrada para a criação do projeto.</param>
    /// <returns>O resultado contendo o projeto criado ou erro.</returns>
    Task<Result<Project>> CreateAsync(CreateProjectInputModel inputModel);

    /// <summary>
    /// Atualiza um projeto existente.
    /// </summary>
    /// <param name="id">O ID do projeto a ser atualizado.</param>
    /// <param name="inputModel">Os dados de entrada para a atualização do projeto.</param>
    /// <returns>O resultado indicando o sucesso ou erro.</returns>
    Task<Result> UpdateAsync(Guid id, UpdateProjectInputModel inputModel);

    /// <summary>
    /// Exclui um projeto.
    /// </summary>
    /// <param name="id">O ID do projeto a ser excluído.</param>
    /// <returns>O resultado indicando o sucesso ou erro.</returns>
    Task<Result> DeleteAsync(Guid id);
}