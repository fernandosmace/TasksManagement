using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Enums;

namespace TasksManagement.Tests.Mocks.Entities;
public static class TaskItemMock
{
    public static TaskItem GetPendingTask()
    {
        return new TaskItem(
            "Tarefa pendente",
            "Descrição da tarefa pendente",
            DateTime.UtcNow.AddDays(1),
            ETaskPriority.Medium,
            Guid.NewGuid());
    }

    public static TaskItem GetCompletedTask()
    {
        var task = new TaskItem(
            "Tarefa concluída",
            "Descrição da tarefa concluída",
            DateTime.UtcNow.AddDays(1),
            ETaskPriority.High,
            Guid.NewGuid());

        task.Update(task.Title, task.Description, task.DueDate, ETaskStatus.Completed);
        return task;
    }

    public static TaskItem GetInProgressTask()
    {
        var task = new TaskItem(
            "Tarefa em andamento",
            "Descrição da tarefa em andamento",
            DateTime.UtcNow.AddDays(1),
            ETaskPriority.Low,
            Guid.NewGuid());

        task.Update(task.Title, task.Description, task.DueDate, ETaskStatus.InProgress);
        return task;
    }

    public static TaskItem GetTaskWithNullTitle()
    {
        return new TaskItem(
            null,
            "Descrição válida",
            DateTime.UtcNow.AddDays(1),
            ETaskPriority.Medium,
            Guid.NewGuid());
    }

    public static TaskItem GetTaskWithEmptyTitle()
    {
        return new TaskItem(
            "",
            "Descrição válida",
            DateTime.UtcNow.AddDays(1),
            ETaskPriority.Medium,
            Guid.NewGuid());
    }

    public static TaskItem GetTaskWithWhitespaceTitle()
    {
        return new TaskItem(
            "   ",
            "Descrição válida",
            DateTime.UtcNow.AddDays(1),
            ETaskPriority.Medium,
            Guid.NewGuid());
    }

    public static TaskItem GetTaskWithNullDescription()
    {
        return new TaskItem(
            "Título válido",
            null,
            DateTime.UtcNow.AddDays(1),
            ETaskPriority.Medium,
            Guid.NewGuid());
    }

    public static TaskItem GetTaskWithEmptyDescription()
    {
        return new TaskItem(
            "Título válido",
            "",
            DateTime.UtcNow.AddDays(1),
            ETaskPriority.Medium,
            Guid.NewGuid());
    }

    public static TaskItem GetTaskWithWhitespaceDescription()
    {
        return new TaskItem(
            "Título válido",
            "   ",
            DateTime.UtcNow.AddDays(1),
            ETaskPriority.Medium,
            Guid.NewGuid());
    }

    public static TaskItem GetTaskWithPastDueDate()
    {
        return new TaskItem(
            "Título válido",
            "Descrição válida",
            DateTime.UtcNow.AddDays(-1),
            ETaskPriority.Medium,
            Guid.NewGuid());
    }
}
