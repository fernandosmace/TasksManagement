using Microsoft.EntityFrameworkCore;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Models.ReportModels;
using TasksManagement.Infrastructure.Database;

namespace TasksManagement.Infrastructure.Repositories;
public class TaskRepository : ITaskRepository
{
    private readonly SqlDbContext _context;

    public TaskRepository(SqlDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TaskItem?>> GetByIdAsync(Guid id)
    {
        try
        {
            var task = await _context.Tasks.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == id);
            return Result.Success(task);
        }
        catch (Exception ex)
        {
            return Result.Failure<TaskItem?>($"Erro ao buscar tarefa: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<TaskItem>>> GetByProjectIdAsync(Guid projectId)
    {
        try
        {
            var tasks = await _context.Tasks.Where(t => t.ProjectId == projectId).ToListAsync();
            return Result.Success<IEnumerable<TaskItem>>(tasks);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<TaskItem>>($"Erro ao buscar tarefas: {ex.Message}");
        }
    }

    public async Task<Result> CreateAsync(TaskItem task)
    {
        try
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return Result.Success("Tarefa criada com sucesso.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao criar tarefa: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(TaskItem task)
    {
        try
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            return Result.Success("Tarefa atualizada com sucesso.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao atualizar tarefa: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(TaskItem task)
    {
        try
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return Result.Success("Tarefa deletada com sucesso.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Erro ao deletar tarefa: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<TaskItem>>> GetCompletedTasksByProjectIdAsync(IEnumerable<Project> projects, DateTime dateThreshold)
    {
        try
        {
            var projectIds = projects.Select(p => p.Id).ToList();
            var tasks = await _context.Tasks
                .Where(t => projectIds.Contains(t.ProjectId) &&
                            t.Status == Domain.Enums.ETaskStatus.Completed &&
                            t.CompletionDate >= dateThreshold)
                .AsNoTracking()
                .ToListAsync();

            return Result.Success<IEnumerable<TaskItem>>(tasks);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<TaskItem>>($"Erro ao buscar tarefas concluídas: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<TaskWithCommentCountReportModel>>> GetTopTasksWithMostCommentsAsync(DateTime dateThreshold)
    {
        try
        {
            var topTasks = await _context.Comments
                .Where(c => c.CreatedAt >= dateThreshold)
                .GroupBy(c => c.TaskId)
                .Select(g => new
                {
                    TaskId = g.Key,
                    CommentCount = g.Count()
                })
                .Join(_context.Tasks,
                    commentGroup => commentGroup.TaskId,
                    task => task.Id,
                    (commentGroup, task) => new TaskWithCommentCountReportModel
                    {
                        TaskId = commentGroup.TaskId,
                        Title = task.Title,
                        CommentCount = commentGroup.CommentCount
                    })
                .OrderByDescending(x => x.CommentCount)
                .Take(10)
                .AsNoTracking()
                .ToListAsync();

            return Result.Success<IEnumerable<TaskWithCommentCountReportModel>>(topTasks);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<TaskWithCommentCountReportModel>>($"Erro ao buscar as tarefas com mais comentários: {ex.Message}");
        }
    }
}