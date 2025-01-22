using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Interfaces.Services;

namespace TasksManagement.Application.Services;
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Cria um novo usuário.
    /// </summary>
    /// <param name="user">O usuário a ser criado.</param>
    /// <returns>O resultado da criação do usuário, incluindo sucesso ou falha.</returns>
    public async Task<Result<User>> CreateAsync(User user)
    {
        try
        {
            var createdUser = await _userRepository.CreateAsync(user);
            if (!createdUser.IsValid)
                return Result.Failure<User>("Erro ao criar o usuário.", statusCode: 500);

            return Result.Success<User>(user);
        }
        catch (Exception ex)
        {
            return Result.Failure<User>("Ocorreu um erro inesperado", statusCode: 500);
        }
    }

    /// <summary>
    /// Obtém um usuário pelo ID.
    /// </summary>
    /// <param name="id">O ID do usuário.</param>
    /// <returns>O usuário encontrado ou erro caso não encontrado.</returns>
    public async Task<Result<User>> GetByIdAsync(Guid id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            return Result.Success<User>(user.Data!);
        }

        catch (Exception ex)
        {
            return Result.Failure<User>("Ocorreu um erro inesperado", statusCode: 500);
        }
    }
}