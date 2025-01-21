using Microsoft.AspNetCore.Mvc;
using TasksManagement.API.Models.InputModels.Project;
using TasksManagement.API.Models.OutputModels.Project;
using TasksManagement.Domain.Interfaces.Services;
using TasksManagement.Domain.Models.OutputModels;
using TasksManagement.Domain.Models.OutputModels.Task;

namespace TasksManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Obtém um projeto pelo ID.
        /// </summary>
        /// <param name="id">O ID do projeto.</param>
        /// <returns>Retorna o projeto ou um erro se não encontrado.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectOutputModel), 200)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 404)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _projectService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? 500, new ErrorResultOutputModel
                (
                    result.Message,
                    result.Notifications.ToList()
                ));
            }

            var projectOutput = new ProjectOutputModel
            {
                Id = result.Data!.Id,
                Name = result.Data.Name,
                UserId = result.Data.UserId,
                Tasks = result.Data.Tasks.Select(task => new TaskOutputModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Status = task.Status,
                    Priority = task.Priority
                }).ToList()
            };

            return Ok(projectOutput);
        }

        /// <summary>
        /// Obtém todos os projetos de um usuário.
        /// </summary>
        /// <param name="userId">O ID do usuário.</param>
        /// <returns>Retorna os projetos ou um erro se não encontrados.</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<ProjectOutputModel>), 200)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 404)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
        public async Task<IActionResult> GetAllByUserIdAsync(Guid userId)
        {
            var result = await _projectService.GetAllByUserIdAsync(userId);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? 500, new ErrorResultOutputModel
                (
                    result.Message,
                    result.Notifications.ToList()
                ));
            }

            var projectOutputs = new List<ProjectOutputModel>();
            foreach (var project in result.Data!)
            {
                projectOutputs.Add(new ProjectOutputModel
                {
                    Id = project.Id,
                    Name = project.Name,
                    UserId = project.UserId,
                    Tasks = project.Tasks.Select(task => new TaskOutputModel
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description,
                        DueDate = task.DueDate,
                        Status = task.Status,
                        Priority = task.Priority
                    }).ToList()
                });
            }

            return Ok(projectOutputs);
        }

        /// <summary>
        /// Obtém todas as tarefas associadas a um projeto.
        /// </summary>
        /// <param name="projectId">O ID do projeto para o qual as tarefas serão retornadas.</param>
        /// <returns>Retorna as tarefas associadas ao projeto ou um erro caso não existam tarefas ou o projeto não seja encontrado.</returns>
        [HttpGet("{projectId}/tasks")]
        [ProducesResponseType(typeof(IEnumerable<TaskOutputModel>), 200)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 404)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
        public async Task<IActionResult> GetTasksByProjectIdAsync(Guid projectId)
        {
            var result = await _projectService.GetTasksByProjectIdAsync(projectId);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? 500, new ErrorResultOutputModel
                (
                    result.Message,
                    result.Notifications.ToList()
                ));
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
                    Status = task.Status,
                    Priority = task.Priority
                });
            }

            return Ok(tasksOutput);
        }

        /// <summary>
        /// Cria um novo projeto. E cria novo usuário, se não existir.
        /// </summary>
        /// <param name="inputModel">Os dados para a criação do projeto.</param>
        /// <returns>Retorna o projeto criado ou um erro se houver falha.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectOutputModel), 201)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 404)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
        public async Task<IActionResult> CreateAsync(CreateProjectInputModel inputModel)
        {
            if (inputModel == null)
                return BadRequest("Dados para criação do projeto não informados.");

            var result = await _projectService.CreateAsync(inputModel);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? 500, new ErrorResultOutputModel
                (
                    result.Message,
                    result.Notifications.ToList()
                ));
            }

            var projectOutput = new ProjectOutputModel
            {
                Id = result.Data!.Id,
                Name = result.Data.Name,
                UserId = result.Data.UserId
            };

            return Created("", projectOutput);
        }

        /// <summary>
        /// Atualiza um projeto existente.
        /// </summary>
        /// <param name="id">O ID do projeto a ser atualizado.</param>
        /// <param name="inputModel">Os dados para a atualização do projeto.</param>
        /// <returns>Retorna sucesso ou erro caso não consiga atualizar o projeto.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 400)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 404)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateProjectInputModel inputModel)
        {
            if (inputModel == null)
                return BadRequest("Dados para atualização do projeto não informados.");

            var result = await _projectService.UpdateAsync(id, inputModel);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? 500, new ErrorResultOutputModel
                (
                    result.Message,
                    result.Notifications.ToList()
                ));
            }

            return NoContent();
        }

        /// <summary>
        /// Exclui um projeto.
        /// </summary>
        /// <param name="id">O ID do projeto a ser excluído.</param>
        /// <returns>Retorna sucesso ou erro caso não consiga excluir o projeto.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 400)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 404)]
        [ProducesResponseType(typeof(ErrorResultOutputModel), 422)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var result = await _projectService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? 500, new ErrorResultOutputModel
                (
                    result.Message,
                    result.Notifications.ToList()
                ));
            }

            return NoContent();
        }
    }
}
