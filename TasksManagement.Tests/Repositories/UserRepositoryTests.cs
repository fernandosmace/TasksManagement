using FluentAssertions;
using Moq;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;

namespace TasksManagement.Tests.Repositories;
public class UserRepositoryTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;

    public UserRepositoryTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_User_When_Id_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("Usuario Teste", "usuario@teste.com");

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id == userId)))
            .ReturnsAsync(Result.Success(user));

        var repository = _mockUserRepository.Object;

        // Act
        var result = await repository.GetByIdAsync(userId);

        // Assert
        result.IsValid.Should().BeTrue("O usuário deve ser retornado com sucesso.");
        result.Data.Should().NotBeNull("O usuário deve ser retornado se o ID existir");
        result.Data.Name.Should().Be("Usuario Teste", "O nome do usuário deve corresponder ao esperado");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_Id_Does_Not_Exist()
    {
        // Arrange
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Failure<User>("Usuário não encontrado"));

        var repository = _mockUserRepository.Object;

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.IsValid.Should().BeFalse("Deve falhar caso o usuário não exista");
        result.Message.Should().Be("Usuário não encontrado", "A mensagem de erro deve ser a esperada.");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_Exception_When_Exception_Occurs()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Erro ao buscar o usuário"));

        var repository = _mockUserRepository.Object;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => repository.GetByIdAsync(userId));

        exception.Message.Should().Be("Erro ao buscar o usuário", "A mensagem de erro deve indicar que houve uma exceção.");
        _mockUserRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once, "O método GetByIdAsync deve ser chamado uma vez");
    }


    [Fact]
    public async Task CreateAsync_Should_Save_User_Successfully()
    {
        // Arrange
        var user = new User("Novo Usuário", "novo@teste.com");

        _mockUserRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(Result.Success("Usuário criado com sucesso."));

        var repository = _mockUserRepository.Object;

        // Act
        var result = await repository.CreateAsync(user);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após salvar o usuário.");
        _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once, "O método CreateAsync deve ser chamado uma vez");
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_Exception_When_Exception_Occurs()
    {
        // Arrange
        var user = new User("Novo Usuário", "novo@teste.com");

        _mockUserRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<User>()))
            .ThrowsAsync(new Exception("Erro ao salvar o usuário"));

        var repository = _mockUserRepository.Object;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => repository.CreateAsync(user));

        exception.Message.Should().Be("Erro ao salvar o usuário", "A mensagem de erro deve indicar que houve uma exceção.");
        _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once, "O método CreateAsync deve ser chamado uma vez");
    }

}
