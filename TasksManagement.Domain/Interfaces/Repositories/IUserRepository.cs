using TasksManagement.Domain.Entities;

namespace TasksManagement.Domain.Interfaces.Repositories;
public interface IUserRepository
{
    Task<Result<User?>> GetByIdAsync(Guid id);
    Task<Result> CreateAsync(User user);
}