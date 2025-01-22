using FluentAssertions;
using TasksManagement.Domain.Entities;
using TasksManagement.Tests.Mocks.Entities;

namespace TasksManagement.Tests.Entities;
public class ProjectTests
{
    [Fact]
    public void Project_Should_Create_Valid_Project()
    {
        var project = ProjectMock.GetDefault();

        project.IsValid.Should().BeTrue("O projeto deve ser válido");
        project.Tasks.Should().BeEmpty("O projeto inicial não deve conter tarefas");
        project.Notifications.Count.Should().Be(0);
    }

    [Fact]
    public void Project_Should_Fail_When_Name_Is_Empty()
    {
        var project = ProjectMock.GetWithEmptyName();

        project.IsValid.Should().BeFalse("O projeto não deve ser válido sem nome");
        project.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(Project.Name) &&
            n.Message == $"Campo {nameof(Project.Name)} não foi informado.");
    }

    [Fact]
    public void Project_Should_Fail_When_Name_Is_Null()
    {
        var project = ProjectMock.GetWithNullName();

        project.IsValid.Should().BeFalse("O projeto não deve ser válido sem nome");
        project.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(Project.Name) &&
            n.Message == $"Campo {nameof(Project.Name)} não foi informado.");
    }

    [Fact]
    public void Project_Should_Fail_When_Name_Is_WhiteSpace()
    {
        var project = ProjectMock.GetWithWhiteSpaceName();

        project.IsValid.Should().BeFalse("O projeto não deve ser válido sem nome");
        project.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(Project.Name) &&
            n.Message == $"Campo {nameof(Project.Name)} não foi informado.");
    }

    [Fact]
    public void Project_Should_Fail_When_There_Are_Pending_Tasks()
    {
        var project = ProjectMock.GetWithPendingTasks();
        project.ValidateForDelete();

        project.IsValid.Should().BeFalse("O projeto não deve ser válido para exclusão com tarefas pendentes");
        project.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(Project.Tasks) &&
            n.Message.Contains("Não é possível remover o projeto enquanto houver tarefas pendentes"));
    }
}
