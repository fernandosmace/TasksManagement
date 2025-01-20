using TasksManagement.Domain.Entities;

namespace TasksManagement.Domain.Interfaces.Repositories;
public interface ICommentRepository
{
    Task CreateAsync(Comment comment);
}