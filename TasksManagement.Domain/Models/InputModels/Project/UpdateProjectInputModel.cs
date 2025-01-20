namespace TasksManagement.API.Models.InputModels.Project;

public record UpdateProjectInputModel
{
    public string? Name { get; set; }

    public UpdateProjectInputModel(string? name)
    {
        Name = name;
    }
}