using System.Diagnostics.CodeAnalysis;
using TasksManagement.Domain.Models.OutputModels.Task;

namespace TasksManagement.API.Models.OutputModels.Project;

[ExcludeFromCodeCoverage]
public record ProjectOutputModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Guid UserId { get; set; }
    public IEnumerable<TaskOutputModel> Tasks { get; set; } = [];
}
