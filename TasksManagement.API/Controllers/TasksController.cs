using Microsoft.AspNetCore.Mvc;
using TasksManagement.API.Models.InputModels.Task;
using TasksManagement.Domain.Interfaces.Services;
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

    [HttpGet("{id}")]
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

    [HttpGet("project/{projectId}")]
    public async Task<ActionResult> GetByProjectIdAsync(Guid projectId)
    {
        var result = await _taskService.GetByProjectIdAsync(projectId);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode ?? 500, new
            {
                message = result.Message,
                errors = result.Notifications
            });
        }

        var tasksOutput = new List<TaskOutputModel>();
        foreach (var task in result.Data!)
        {
            tasksOutput.Add(new TaskOutputModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status,
                Comments = task.Comments.Select(comment => new CommentOutputModel
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt,
                    UserId = comment.UserId
                }).ToList()
            });
        }

        return Ok(tasksOutput);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateTaskInputModel inputModel)
    {
        if (inputModel == null)
            return BadRequest("Dados para criação do projeto não informados.");

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

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
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