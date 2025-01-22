using FluentAssertions;
using TasksManagement.Domain.Entities;
using TasksManagement.Tests.Mocks.Entities;

namespace TasksManagement.Tests.Entities;
public class TaskItemTests
{
    [Fact]
    public void TaskItem_Should_Fail_When_Title_Is_Null()
    {
        // Arrange
        var task = TaskItemMock.GetTaskWithNullTitle();

        // Assert
        task.IsValid.Should().BeFalse("A tarefa não deve ser válida com título nulo");
        task.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(TaskItem.Title) &&
            n.Message == $"Campo {nameof(TaskItem.Title)} não foi informado.");
    }

    [Fact]
    public void TaskItem_Should_Fail_When_Title_Is_Empty()
    {
        // Arrange
        var task = TaskItemMock.GetTaskWithEmptyTitle();

        // Assert
        task.IsValid.Should().BeFalse("A tarefa não deve ser válida com título vazio");
        task.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(TaskItem.Title) &&
            n.Message == $"Campo {nameof(TaskItem.Title)} não foi informado.");
    }

    [Fact]
    public void TaskItem_Should_Fail_When_Title_Is_Whitespace()
    {
        // Arrange
        var task = TaskItemMock.GetTaskWithWhitespaceTitle();

        // Assert
        task.IsValid.Should().BeFalse("A tarefa não deve ser válida com título em branco");
        task.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(TaskItem.Title) &&
            n.Message == $"Campo {nameof(TaskItem.Title)} não foi informado.");
    }

    [Fact]
    public void TaskItem_Should_Fail_When_Description_Is_Null()
    {
        // Arrange
        var task = TaskItemMock.GetTaskWithNullDescription();

        // Assert
        task.IsValid.Should().BeFalse("A tarefa não deve ser válida com descrição nula");
        task.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(TaskItem.Description) &&
            n.Message == $"Campo {nameof(TaskItem.Description)} não foi informado.");
    }

    [Fact]
    public void TaskItem_Should_Fail_When_Description_Is_Empty()
    {
        // Arrange
        var task = TaskItemMock.GetTaskWithEmptyDescription();

        // Assert
        task.IsValid.Should().BeFalse("A tarefa não deve ser válida com descrição vazia");
        task.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(TaskItem.Description) &&
            n.Message == $"Campo {nameof(TaskItem.Description)} não foi informado.");
    }

    [Fact]
    public void TaskItem_Should_Fail_When_Description_Is_Whitespace()
    {
        // Arrange
        var task = TaskItemMock.GetTaskWithWhitespaceDescription();

        // Assert
        task.IsValid.Should().BeFalse("A tarefa não deve ser válida com descrição em branco");
        task.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(TaskItem.Description) &&
            n.Message == $"Campo {nameof(TaskItem.Description)} não foi informado.");
    }

    [Fact]
    public void TaskItem_Should_Fail_When_DueDate_Is_In_The_Past()
    {
        // Arrange
        var task = TaskItemMock.GetTaskWithPastDueDate();

        // Assert
        task.IsValid.Should().BeFalse("A tarefa não deve ser válida com data no passado.");
        task.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(TaskItem.DueDate) &&
            n.Message == $"Campo {nameof(TaskItem.DueDate)} deve ser uma data futura.");
    }
}