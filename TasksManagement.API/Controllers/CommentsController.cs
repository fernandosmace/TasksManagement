using Microsoft.AspNetCore.Mvc;
using TasksManagement.API.Models.InputModels.Comment;
using TasksManagement.Domain.Interfaces.Services;
using TasksManagement.Domain.Models.OutputModels.Comments;

namespace TasksManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly ICommentService _commentService;

        public CommentsController(ITaskService taskService, IUserService userService, ICommentService commentService)
        {
            _taskService = taskService;
            _userService = userService;
            _commentService = commentService;
        }

        /// <summary>
        /// Cria um novo comentário.
        /// </summary>
        /// <param name="inputModel">O modelo de entrada contendo os dados do comentário a ser criado.</param>
        /// <returns>Retorna o comentário criado ou erro se houver falha na criação.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CommentWithTaskOutputModel), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> Create(CreateCommentInputModel inputModel)
        {
            if (inputModel == null)
                return BadRequest("Dados para criação do comentário não informados.");

            var result = await _commentService.CreateAsync(inputModel);

            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? 500, new
                {
                    message = result.Message,
                    errors = result.Notifications
                });
            }

            var commentOutput = new CommentWithTaskOutputModel
            {
                Id = result.Data!.Id,
                TaskId = result.Data.TaskId,
                Content = result.Data.Content,
                UserId = result.Data.UserId,
                CreatedAt = result.Data.CreatedAt
            };

            return Created("", commentOutput);
        }
    }
}
