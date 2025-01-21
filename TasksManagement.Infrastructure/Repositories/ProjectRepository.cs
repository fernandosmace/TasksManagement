using Microsoft.EntityFrameworkCore;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Models.ReportModels;
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

    public async Task<IEnumerable<ProjectWithTasksCountReportModel>> GetTopProjectsWithMostCompletedTasksAsync(int days = 30)
    {
        var topProjects = await _context.Tasks
            .Where(t => t.Status == Domain.Enums.ETaskStatus.Completed && t.CompletionDate >= DateTime.UtcNow.AddDays(-days))
            .GroupBy(t => t.ProjectId)
            .Select(g => new
            {
                ProjectId = g.Key,
                CompletedTasksCount = g.Count()
            })
            .Join(_context.Projects,
                taskGroup => taskGroup.ProjectId,
                project => project.Id,
                (taskGroup, project) => new ProjectWithTasksCountReportModel
                {
                    Id = taskGroup.ProjectId,
                    Name = project.Name,
                    TasksCount = taskGroup.CompletedTasksCount
                })
            .OrderByDescending(p => p.TasksCount)
            .Take(10)
            .ToListAsync();

        return topProjects;
    }
}