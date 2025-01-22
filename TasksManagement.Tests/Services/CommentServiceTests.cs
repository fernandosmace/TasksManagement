using FluentAssertions;
using Moq;
using TasksManagement.API.Models.InputModels.Comment;
using TasksManagement.API.Models.InputModels.User;
using TasksManagement.Application.Services;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Interfaces.Services;
using TasksManagement.Tests.Mocks.Entities;

namespace TasksManagement.Tests.Services
{
    public class CommentServiceTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ICommentRepository> _mockCommentRepository;
        private readonly Mock<ITaskHistoryRepository> _mockTaskHistoryRepository;

        public CommentServiceTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _mockUserService = new Mock<IUserService>();
            _mockCommentRepository = new Mock<ICommentRepository>();
            _mockTaskHistoryRepository = new Mock<ITaskHistoryRepository>();
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Task_Not_Found()
        {
            // Arrange
            var inputModel = new CreateCommentInputModel(
                Guid.NewGuid(),
                "Test Comment",
                DateTime.UtcNow,
                new UserInputModel(Guid.NewGuid(), "Usuário", "Gerente")
            );

            _mockTaskService
                .Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Failure<TaskItem>());

            var service = new CommentService(_mockTaskService.Object, _mockUserService.Object, _mockCommentRepository.Object, _mockTaskHistoryRepository.Object);

            // Act
            var result = await service.CreateAsync(inputModel);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Message.Should().Contain("Tarefa não encontrado.");
            _mockTaskService.Verify(service => service.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_User_Not_Found()
        {
            // Arrange
            var inputModel = new CreateCommentInputModel(
                Guid.NewGuid(),
                "Test Comment",
                DateTime.UtcNow,
                new UserInputModel(Guid.NewGuid(), "Usuário", "Gerente")
            );

            var task = TaskItemMock.GetPendingTask();
            var user = new User("Test User", "test@example.com");

            _mockTaskService
                .Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Success(task));

            _mockUserService
                .Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Failure<User>());

            var service = new CommentService(_mockTaskService.Object, _mockUserService.Object, _mockCommentRepository.Object, _mockTaskHistoryRepository.Object);

            // Act
            var result = await service.CreateAsync(inputModel);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Message.Should().Contain("Úsuário não encontrado.");
            _mockUserService.Verify(service => service.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Create_Comment_Fails()
        {
            // Arrange
            var inputModel = new CreateCommentInputModel(
                Guid.NewGuid(),
                "Test Comment",
                DateTime.UtcNow,
                new UserInputModel(Guid.NewGuid(), "Usuário", "Gerente")
            );

            var task = TaskItemMock.GetPendingTask();
            var user = new User("Test User", "test@example.com");

            var failedResult = Result.Failure("Falha ao criar o Comentário.", statusCode: 500);

            var comment = new Comment(inputModel.Content, inputModel.TaskId, inputModel.User.Id);

            _mockTaskService
                .Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Success(task));

            _mockUserService
                .Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Success(user));



            _mockCommentRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<Comment>()))
                .ReturnsAsync(failedResult);

            var service = new CommentService(_mockTaskService.Object, _mockUserService.Object, _mockCommentRepository.Object, _mockTaskHistoryRepository.Object);

            // Act
            var result = await service.CreateAsync(inputModel);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Message.Should().Contain("Falha ao criar o Comentário.");
            _mockCommentRepository.Verify(repo => repo.CreateAsync(It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Comment_When_Valid()
        {
            // Arrange
            var inputModel = new CreateCommentInputModel(
                Guid.NewGuid(),
                "Test Comment",
                DateTime.UtcNow,
                new UserInputModel(Guid.NewGuid(), "Usuário", "Gerente")
            );

            var task = TaskItemMock.GetPendingTask();
            var user = new User("Test User", "test@example.com");
            var comment = new Comment(inputModel.Content, inputModel.TaskId, inputModel.User.Id);

            _mockTaskService
                .Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Success(task));

            _mockUserService
                .Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Result.Success(user));

            _mockCommentRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<Comment>()))
                .ReturnsAsync(Result.Success());

            _mockTaskHistoryRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<TaskHistory>()))
                .ReturnsAsync(Result.Success());

            var service = new CommentService(_mockTaskService.Object, _mockUserService.Object, _mockCommentRepository.Object, _mockTaskHistoryRepository.Object);

            // Act
            var result = await service.CreateAsync(inputModel);

            // Assert
            result.IsValid.Should().BeTrue();
            _mockCommentRepository.Verify(repo => repo.CreateAsync(It.IsAny<Comment>()), Times.Once);
            _mockTaskHistoryRepository.Verify(repo => repo.CreateAsync(It.IsAny<TaskHistory>()), Times.Once);
        }
    }
}
