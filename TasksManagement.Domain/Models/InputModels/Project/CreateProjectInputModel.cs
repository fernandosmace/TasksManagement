using System.ComponentModel.DataAnnotations;
using TasksManagement.API.Models.InputModels.User;

namespace TasksManagement.API.Models.InputModels.Project;

public record CreateProjectInputModel
{

    [Required(ErrorMessage = "Usuário não informado")]
    public UserInputModel User { get; set; }

    [Required(ErrorMessage = "Nome do Projeto não informado.")]
    public string? Name { get; set; }

    public CreateProjectInputModel(UserInputModel user, string? name)
    {
        User = user;
        Name = name;
    }
}