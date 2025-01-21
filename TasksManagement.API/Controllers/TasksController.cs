using Microsoft.AspNetCore.Mvc;
using TasksManagement.API.Models.InputModels.Task;
using TasksManagement.Domain.Interfaces.Services;
using TasksManagement.Domain.Models.OutputModels;
using TasksManagement.Domain.Models.OutputModels.Comments;
using TasksManagement.Domain.Models.OutputModels.Task;

namespace TasksManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;

    public TasksController(IProjectService projectService, ITaskService taskService, IUserService userService)
    {
        _projectService = projectService;
        _taskService = taskService;
        _userService = userService;
    }

    /// <summary>
    /// Obtém uma tarefa pelo ID.
    /// </summary>
    /// <param name="id">O ID da tarefa.</param>
    /// <returns>Retorna a tarefa ou um erro se não encontrada.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskWithProjectOutputModel), 200)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 404)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var result = await _taskService.GetByIdAsync(id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode ?? 500, new
            {
                message = result.Message,
                errors = result.Notifications
            });
        }

        var taskOutput = new TaskWithProjectOutputModel
        {
            Id = result.Data!.Id,
            Title = result.Data!.Title,
            Description = result.Data!.Description,
            DueDate = result.Data!.DueDate,
            Priority = result.Data!.Priority,
            Status = result.Data!.Status,
            ProjectId = result.Data!.ProjectId,
            Comments = result.Data!.Comments.Select(comment => new CommentOutputModel
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UserId = comment.UserId
            }).ToList()
        };

        return Ok(taskOutput);
    }

    /// <summary>
    /// Cria uma nova tarefa.
    /// </summary>
    /// <param name="inputModel">Os dados para a criação da tarefa.</param>
    /// <returns>Retorna a tarefa criada ou um erro se houver falha.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TaskWithProjectOutputModel), 201)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 400)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
    public async Task<IActionResult> CreateAsync(CreateTaskInputModel inputModel)
    {
        if (inputModel == null)
            return BadRequest("Dados para criação da tarefa não informados.");

        var result = await _taskService.CreateAsync(inputModel);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode ?? 500, new
            {
                message = result.Message,
                errors = result.Notifications
            });
        }

        var taskOutput = new TaskWithProjectOutputModel
        {
            Id = result.Data!.Id,
            Title = result.Data!.Title,
            Description = result.Data!.Description,
            DueDate = result.Data!.DueDate,
            Priority = result.Data!.Priority,
            ProjectId = result.Data!.ProjectId,
            Status = result.Data!.Status
        };

        return Created("", taskOutput);
    }

    /// <summary>
    /// Atualiza uma tarefa existente.
    /// </summary>
    /// <param name="id">O ID da tarefa a ser atualizada.</param>
    /// <param name="inputModel">Os dados para a atualização da tarefa.</param>
    /// <returns>Retorna sucesso ou erro caso não consiga atualizar a tarefa.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 400)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 404)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateTaskInputModel inputModel)
    {
        if (inputModel == null)
            return BadRequest("Dados para atualização da tarefa não informados.");

        var result = await _taskService.UpdateAsync(id, inputModel);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode ?? 500, new
            {
                message = result.Message,
                errors = result.Notifications
            });
        }

        return NoContent();
    }

    /// <summary>
    /// Exclui uma tarefa.
    /// </summary>
    /// <param name="id">O ID da tarefa a ser excluída.</param>
    /// <returns>Retorna sucesso ou erro caso não consiga excluir a tarefa.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 400)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 404)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _taskService.DeleteAsync(id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode ?? 500, new
            {
                message = result.Message,
                errors = result.Notifications
            });
        }

        return NoContent();
    }
}
