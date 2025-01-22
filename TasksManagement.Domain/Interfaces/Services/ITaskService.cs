using TasksManagement.API.Models.InputModels.Task;
using TasksManagement.Domain.Entities;

namespace TasksManagement.Domain.Interfaces.Services;
public interface ITaskService
{
    /// <summary>
    /// Obtém uma tarefa pelo ID.
    /// </summary>
    /// <param name="id">O ID da tarefa.</param>
    /// <returns>Retorna o resultado contendo a tarefa ou erro.</returns>
    Task<Result<TaskItem>> GetByIdAsync(Guid id);

    /// <summary>
    /// Cria uma nova tarefa.
    /// </summary>
    /// <param name="inputModel">Os dados de entrada para criação da tarefa.</param>
    /// <returns>O resultado contendo a tarefa criada ou erro.</returns>
    Task<Result<TaskItem>> CreateAsync(CreateTaskInputModel inputModel);

    /// <summary>
    /// Atualiza uma tarefa existente.
    /// </summary>
    /// <param name="id">O ID da tarefa a ser atualizada.</param>
    /// <param name="inputModel">Os dados de entrada para a atualização da tarefa.</param>
    /// <returns>O resultado indicando o sucesso ou erro.</returns>
    Task<Result> UpdateAsync(Guid id, UpdateTaskInputModel inputModel);

    /// <summary>
    /// Exclui uma tarefa.
    /// </summary>
    /// <param name="id">O ID da tarefa a ser excluída.</param>
    /// <returns>O resultado indicando o sucesso ou erro.</returns>
    Task<Result> DeleteAsync(Guid id);
}