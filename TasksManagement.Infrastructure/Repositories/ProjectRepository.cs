using Microsoft.EntityFrameworkCore;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Infrastructure.Database;

namespace TasksManagement.Infrastructure.Repositories;
public class ProjectRepository : IProjectRepository
{
    private readonly SqlDbContext _context;

    public ProjectRepository(SqlDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(Guid id) => await _context.Projects.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Project>> GetAllByUserIdAsync(Guid userId) => await _context.Projects.Include(p => p.Tasks).Where(p => p.UserId == userId).AsNoTracking().ToListAsync();

    public async Task CreateAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }
}