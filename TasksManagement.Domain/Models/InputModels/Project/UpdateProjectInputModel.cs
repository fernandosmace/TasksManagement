using System.Diagnostics.CodeAnalysis;

namespace TasksManagement.API.Models.InputModels.Project;

[ExcludeFromCodeCoverage]
public record UpdateProjectInputModel
{
    public string? Name { get; set; }

    public UpdateProjectInputModel(string? name)
    {
        Name = name;
    }
}