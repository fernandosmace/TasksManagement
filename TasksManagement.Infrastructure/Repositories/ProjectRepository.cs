using Microsoft.EntityFrameworkCore;
using TasksManagement.Domain;
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

    public async Task<Result<Project>> GetByIdAsync(Guid id)
    {
        try
        {
            var project = await _context
                            .Projects
                            .Include(p => p.Tasks)
                            .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return Result.Failure<Project>("Projeto não encontrado.");

            return Result.Success(project);
        }
        catch (Exception ex)
        {
            return Result.Failure<Project>($"Erro ao buscar o projeto: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Project>>> GetAllByUserIdAsync(Guid userId)
    {
        try
        {
            var projects = await _context.Projects
                                    .Include(p => p.Tasks)
                                    .Where(p => p.UserId == userId)
                                    .AsNoTracking()
                                    .ToListAsync();

            return Result.Success((IEnumerable<Project>)projects);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<Project>>($"Erro ao buscar projetos: {ex.Message}");
        }
    }

    public async Task<Result> CreateAsync(Project project)
    {
        try
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return Result.Success("Projeto criado com sucesso.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao criar o projeto: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Project project)
    {
        try
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
            return Result.Success("Projeto atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao atualizar o projeto: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Project project)
    {
        try
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return Result.Success("Projeto deletado com sucesso.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao deletar o projeto: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<ProjectWithTasksCountReportModel>>> GetTopProjectsWithMostCompletedTasksAsync(int days = 30)
    {
        try
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
                .AsNoTracking()
                .ToListAsync();

            return Result.Success((IEnumerable<ProjectWithTasksCountReportModel>)topProjects);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<ProjectWithTasksCountReportModel>>($"Erro ao buscar projetos: {ex.Message}");
        }
    }
}