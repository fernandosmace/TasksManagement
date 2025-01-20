using TasksManagement.API.Models.InputModels.Comment;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Interfaces.Services;

namespace TasksManagement.Application.Services;
public class CommentService : ICommentService
{
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;
    private readonly ICommentRepository _commentRepository;
    private readonly ITaskHistoryRepository _taskHistoryRepository;

    public CommentService(ITaskService taskService, IUserService userService, ICommentRepository commentRepository, ITaskHistoryRepository taskHistoryRepository)
    {
        _taskService = taskService;
        _userService = userService;
        _commentRepository = commentRepository;
        _taskHistoryRepository = taskHistoryRepository;
    }

    public async Task<Result<Comment>> CreateAsync(CreateCommentInputModel inputModel)
    {
        try
        {
            var task = await _taskService.GetByIdAsync(inputModel.TaskId);
            if (task == null || task.Data == null)
                return Result.Failure<Comment>("Tarefa não encontrado.", statusCode: 404);

            var user = await _userService.GetByIdAsync(inputModel.User.Id);
            if (user == null || user.Data == null)
                return Result.Failure<Comment>(message: "Úsuário não encontrado.", statusCode: 404);

            var comment = new Comment(inputModel.Content!, task.Data!.Id, user.Data.Id);

            await _commentRepository.CreateAsync(comment);
            await _taskHistoryRepository.CreateAsync(
                new TaskHistory(
                    $"Comentário adicionado: '{comment.Content}'",
                    comment.TaskId,
                    user.Data.Id));

            return Result.Success(comment);
        }
        catch (Exception ex)
        {
            return Result.Failure<Comment>("Ocorreu um erro inesperado", statusCode: 500);
        }
    }
}