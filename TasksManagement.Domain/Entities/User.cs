using Flunt.Validations;

namespace TasksManagement.Domain.Entities;
public class User : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public User(string name, string role)
    {
        Name = name;
        Role = role;

        AddNotifications(new Contract<User>()
            .Requires()
            .IsNotNullOrWhiteSpace(Name, nameof(Name), $"Campo {nameof(Name)} não foi informado")
            .IsNotNullOrWhiteSpace(Role, nameof(Role), $"Campo {nameof(Role)} não foi informado")
        );
    }
}