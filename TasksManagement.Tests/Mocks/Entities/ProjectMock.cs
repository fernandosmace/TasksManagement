using TasksManagement.Domain.Entities;

namespace TasksManagement.Tests.Mocks.Entities;
public static class ProjectMock
{
    public static Project GetDefault() => new Project("Projeto válido", Guid.NewGuid());

    public static Project GetWithEmptyName() => new Project("", Guid.NewGuid());

    public static Project GetWithNullName() => new Project(null, Guid.NewGuid());
    public static Project GetWithWhiteSpaceName() => new Project(" ", Guid.NewGuid());

    public static Project GetWithPendingTasks()
    {
        var project = new Project("Projeto com tarefas pendentes", Guid.NewGuid());
        var tasks = new List<TaskItem>
        {
            TaskItemMock.GetPendingTask()
        };

        SetPrivateField(project, "_tasks", tasks);
        return project;
    }

    public static Project GetWithoutPendingTasks()
    {
        var project = new Project("Projeto sem tarefas pendentes", Guid.NewGuid());
        var tasks = new List<TaskItem>
        {
            TaskItemMock.GetCompletedTask()
        };

        SetPrivateField(project, "_tasks", tasks);
        return project;
    }

    private static void SetPrivateField<T>(T instance, string fieldName, object value)
    {
        var fieldInfo = typeof(T).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        fieldInfo?.SetValue(instance, value);
    }
}