using FluentAssertions;
using TasksManagement.Domain.Entities;
using TasksManagement.Tests.Mocks.Entities;

namespace TasksManagement.Tests.Entities;
public class UserTests
{
    [Fact]
    public void User_Should_Create_Valid_User()
    {
        // Arrange & Act
        var user = UserMock.GetDefault();

        // Assert
        user.IsValid.Should().BeTrue("O usuário deve ser válido");
        user.Name.Should().NotBeNullOrWhiteSpace("O nome deve estar preenchido");
        user.Role.Should().NotBeNullOrWhiteSpace("O papel deve estar preenchido");
        user.Projects.Should().BeEmpty("O usuário padrão não deve ter projetos associados inicialmente");
        user.Notifications.Count.Should().Be(0);
    }

    [Fact]
    public void User_Should_Fail_When_Name_Is_Empty()
    {
        // Arrange & Act
        var user = UserMock.GetWithEmptyName();

        // Assert
        user.IsValid.Should().BeFalse("O usuário não deve ser válido sem nome");
        user.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(User.Name) &&
            n.Message == $"Campo {nameof(User.Name)} não foi informado");
    }

    [Fact]
    public void User_Should_Fail_When_Name_Is_Null()
    {
        // Arrange & Act
        var user = UserMock.GetWithNullName();

        // Assert
        user.IsValid.Should().BeFalse("O usuário não deve ser válido sem nome");
        user.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(User.Name) &&
            n.Message == $"Campo {nameof(User.Name)} não foi informado");
    }

    [Fact]
    public void User_Should_Fail_When_Name_Is_WhiteSpace()
    {
        // Arrange & Act
        var user = UserMock.GetWithWhiteSpaceName();

        // Assert
        user.IsValid.Should().BeFalse("O usuário não deve ser válido sem nome");
        user.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(User.Name) &&
            n.Message == $"Campo {nameof(User.Name)} não foi informado");
    }

    [Fact]
    public void User_Should_Fail_When_Role_Is_Empty()
    {
        // Arrange & Act
        var user = UserMock.GetWithEmptyRole();

        // Assert
        user.IsValid.Should().BeFalse("O usuário não deve ser válido sem papel definido");
        user.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(User.Role) &&
            n.Message == $"Campo {nameof(User.Role)} não foi informado");
    }

    [Fact]
    public void User_Should_Fail_When_Role_Is_Null()
    {
        // Arrange & Act
        var user = UserMock.GetWithNullRole();

        // Assert
        user.IsValid.Should().BeFalse("O usuário não deve ser válido sem papel definido");
        user.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(User.Role) &&
            n.Message == $"Campo {nameof(User.Role)} não foi informado");
    }

    [Fact]
    public void User_Should_Fail_When_Role_Is_WhiteSpace()
    {
        // Arrange & Act
        var user = UserMock.GetWithWhiteSpaceRole();

        // Assert
        user.IsValid.Should().BeFalse("O usuário não deve ser válido sem papel definido");
        user.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(User.Role) &&
            n.Message == $"Campo {nameof(User.Role)} não foi informado");
    }
}
