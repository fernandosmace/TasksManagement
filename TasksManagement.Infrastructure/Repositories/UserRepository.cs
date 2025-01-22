using Microsoft.EntityFrameworkCore;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Infrastructure.Database;

namespace TasksManagement.Infrastructure.Repositories;
public class UserRepository : IUserRepository
{
    private readonly SqlDbContext _context;

    public UserRepository(SqlDbContext context)
    {
        _context = context;
    }
    public async Task<Result<User?>> GetByIdAsync(Guid id)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(t => t.Id == id);
            return Result.Success(user);
        }
        catch (Exception ex)
        {
            return Result.Failure<User?>($"Erro ao buscar usuário: {ex.Message}");
        }
    }
    public async Task<Result> CreateAsync(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {

            return Result.Failure($"Erro ao criar usuário: {ex.Message}");
        }
    }
}