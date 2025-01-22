using System.Diagnostics.CodeAnalysis;

namespace TasksManagement.API.Models.InputModels.User;

[ExcludeFromCodeCoverage]
public record UserInputModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }

    public UserInputModel(Guid id, string? name, string? role)
    {
        Id = id;
        Name = name;
        Role = role;
    }
}
