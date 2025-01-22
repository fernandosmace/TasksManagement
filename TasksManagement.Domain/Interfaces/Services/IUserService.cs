using TasksManagement.Domain.Entities;

namespace TasksManagement.Domain.Interfaces.Services;
/// <summary>
/// Interface que define os métodos para manipulação de usuários.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Obtém um usuário pelo ID.
    /// </summary>
    /// <param name="id">O ID do usuário.</param>
    /// <returns>Retorna o usuário ou erro se não encontrado.</returns>
    Task<Result<User>> GetByIdAsync(Guid id);

    /// <summary>
    /// Cria um novo usuário.
    /// </summary>
    /// <param name="user">O objeto contendo os dados do usuário a ser criado.</param>
    /// <returns>Retorna o usuário criado ou erro se houver falha na criação.</returns>
    Task<Result<User>> CreateAsync(User user);
}