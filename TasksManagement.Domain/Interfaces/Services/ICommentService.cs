using TasksManagement.API.Models.InputModels.Comment;
using TasksManagement.Domain.Entities;

namespace TasksManagement.Domain.Interfaces.Services
{
    /// <summary>
    /// Interface que define os métodos para manipulação de comentários.
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// Cria um novo comentário.
        /// </summary>
        /// <param name="inputModel">O modelo de entrada contendo os dados do comentário a ser criado.</param>
        /// <returns>Retorna o comentário criado ou erro se houver falha na criação.</returns>
        Task<Result<Comment>> CreateAsync(CreateCommentInputModel inputModel);
    }
}
