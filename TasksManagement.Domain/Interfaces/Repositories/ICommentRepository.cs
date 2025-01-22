using TasksManagement.Domain.Entities;

namespace TasksManagement.Domain.Interfaces.Repositories;
public interface ICommentRepository
{
    Task<Result> CreateAsync(Comment comment);
}