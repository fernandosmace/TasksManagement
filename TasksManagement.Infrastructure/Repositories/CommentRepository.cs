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

    public async Task CreateAsync(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }
}