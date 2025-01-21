using Microsoft.AspNetCore.Mvc;
using TasksManagement.API.Models.OutputModels.Task;
using TasksManagement.Domain.Interfaces.Services;
using TasksManagement.Domain.Models.OutputModels;
using TasksManagement.Domain.Models.OutputModels.Project;

[ApiController]
[Route("api/reports")]
public class TaskReportController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITaskReportService _taskReportService;
    private readonly IProjectReportService _projectReportService;

    public TaskReportController(ITaskReportService taskReportService, IProjectReportService projectReportService, IUserService userService)
    {
        _taskReportService = taskReportService;
        _projectReportService = projectReportService;
        _userService = userService;
    }

    /// <summary>
    /// Gera um relatório de tarefas completadas por um usuário.
    /// </summary>
    /// <param name="userId">O ID do usuário para o qual o relatório será gerado.</param>
    /// <param name="userRequestId">O ID do usuário solicitante.</param>
    /// <param name="days">O número de dias para considerar no relatório (máximo: 30).</param>
    /// <returns>Retorna o relatório ou um erro de permissão/validação.</returns>
    [HttpGet("users/{userId}/tasks/{days}")]
    [ProducesResponseType(typeof(IEnumerable<TaskReportModel>), 200)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 401)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
    public async Task<IActionResult> GetTasksCompletedByUserAsync(Guid userId, Guid userRequestId, int days)
    {
        var user = await _userService.GetByIdAsync(userRequestId);
        if (user == null || user.Data == null || !user.Data.Role.Equals("Gerente"))
            return Unauthorized("Usuário sem permissão para geração de relatórios.");

        if (days > 30)
            return UnprocessableEntity("Não é possível geração de relatórios para período superior a 30 dias.");

        var result = await _taskReportService.GenerateCompletedTasksByUserReportAsync(userId, days);
        return Ok(result.Data);
    }

    /// <summary>
    /// Gera um relatório das 10 tarefas com mais comentários.
    /// </summary>
    /// <param name="userRequestId">O ID do usuário solicitante.</param>
    /// <param name="days">O número de dias para considerar no relatório (máximo: 30).</param>
    /// <returns>Retorna o relatório ou um erro de permissão/validação.</returns>
    [HttpGet("top/tasks-with-most-comments/{days}")]
    [ProducesResponseType(typeof(IEnumerable<TaskReportModel>), 200)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 401)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
    public async Task<IActionResult> GetTasksWithMostCommentsAsync(Guid userRequestId, int days)
    {
        var user = await _userService.GetByIdAsync(userRequestId);
        if (user == null || user.Data == null || !user.Data.Role.Equals("Gerente"))
            return Unauthorized("Usuário sem permissão para geração de relatórios.");

        if (days > 30)
            return UnprocessableEntity("Não é possível geração de relatórios para período superior a 30 dias.");

        var result = await _taskReportService.GenerateTopTasksByCommentsAsync(days);
        return Ok(result.Data);
    }

    /// <summary>
    /// Gera um relatório dos 10 projetos com mais tarefas completadas.
    /// </summary>
    /// <param name="userRequestId">O ID do usuário solicitante.</param>
    /// <param name="days">O número de dias para considerar no relatório (máximo: 30).</param>
    /// <returns>Retorna o relatório ou um erro de permissão/validação.</returns>
    [HttpGet("top/projects-with-completed-tasks/{days}")]
    [ProducesResponseType(typeof(IEnumerable<ProjectReportModel>), 200)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 401)]
    [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
    public async Task<IActionResult> GetProjectsWithMostCompletedTasksAsync(Guid userRequestId, int days)
    {
        var user = await _userService.GetByIdAsync(userRequestId);
        if (user == null || user.Data == null || !user.Data.Role.Equals("Gerente"))
            return Unauthorized("Usuário sem permissão para geração de relatórios.");

        if (days > 30)
            return UnprocessableEntity("Não é possível geração de relatórios para período superior a 30 dias.");

        var result = await _projectReportService.GenerateTopProjectsByCompletedTasksAsync(days);
        return Ok(result.Data);
    }
}