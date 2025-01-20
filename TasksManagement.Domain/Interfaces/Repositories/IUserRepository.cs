using TasksManagement.Domain.Entities;

namespace TasksManagement.Domain.Interfaces.Repositories;
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task CreateAsync(User user);
}