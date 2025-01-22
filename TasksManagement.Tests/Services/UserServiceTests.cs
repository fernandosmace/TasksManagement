using FluentAssertions;
using Moq;
using TasksManagement.Application.Services;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;

namespace TasksManagement.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _service = new UserService(_mockUserRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Success_When_User_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("Test User", "Role");
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(Result.Success(user));

        // Act
        var result = await _service.GetByIdAsync(userId);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().Be(user);
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_True_When_User_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(Result.Success((User)null));

        // Act
        var result = await _service.GetByIdAsync(userId);

        // Assert
        result.IsValid.Should().BeTrue();
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Success_When_User_Created()
    {
        // Arrange
        var user = new User("Test User", "Role");
        _mockUserRepository
            .Setup(repo => repo.CreateAsync(user))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _service.CreateAsync(user);

        // Assert
        result.IsValid.Should().BeTrue();
        _mockUserRepository.Verify(repo => repo.CreateAsync(user), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Creation_Fails()
    {
        // Arrange
        var user = new User("Test User", "Role");
        _mockUserRepository
            .Setup(repo => repo.CreateAsync(user))
            .ReturnsAsync(Result.Failure("Erro ao criar o usuário.", statusCode: 500));

        // Act
        var result = await _service.CreateAsync(user);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Erro ao criar o usuário.");
        _mockUserRepository.Verify(repo => repo.CreateAsync(user), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var user = new User("Test User", "Role");
        _mockUserRepository
            .Setup(repo => repo.CreateAsync(user))
            .ThrowsAsync(new Exception("Erro desconhecido."));

        // Act
        var result = await _service.CreateAsync(user);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Ocorreu um erro inesperado");
        _mockUserRepository.Verify(repo => repo.CreateAsync(user), Times.Once);
    }
}

