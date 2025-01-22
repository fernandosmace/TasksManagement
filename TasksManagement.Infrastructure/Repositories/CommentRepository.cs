using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Infrastructure.Database;

namespace TasksManagement.Infrastructure.Repositories;
public class CommentRepository : ICommentRepository
{
    private readonly SqlDbContext _context;

    public CommentRepository(SqlDbContext context)
    {
        _context = context;
    }

    public async Task<Result> CreateAsync(Comment comment)
    {
        try
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch
        {
            return Result.Failure();
        }
    }
}