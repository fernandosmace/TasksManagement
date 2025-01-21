using Microsoft.EntityFrameworkCore;
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

    public async Task<TaskItem?> GetByIdAsync(Guid id) => await _context.Tasks.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId) => await _context.Tasks.Where(t => t.ProjectId == projectId).ToListAsync();

    public async Task CreateAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetCompletedTasksByProjectIdAsync(IEnumerable<Project> projects, DateTime dateThreshold)
    {
        var projectIds = projects.Select(p => p.Id).ToList();

        return await _context.Tasks
            .Where(t => projectIds.Contains(t.ProjectId) &&
                         t.Status == Domain.Enums.ETaskStatus.Completed &&
                         t.CompletionDate >= dateThreshold)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskWithCommentCountReportModel>> GetTopTasksWithMostCommentsAsync(DateTime dateThreshold)
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
            .ToListAsync();

        return topTasks;
    }
}