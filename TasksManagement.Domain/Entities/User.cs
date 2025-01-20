using Flunt.Validations;

namespace TasksManagement.Domain.Entities;
public class User : Entity
{
    public string Name { get; set; }
    public string Role { get; set; }
    public virtual ICollection<Project> Projects { get; private set; } = [];

    public User(string name, string role)
    {
        Name = name;
        Role = role;

        Validar();
    }

    protected override void Validar()
    {
        AddNotifications(new Contract<User>()
            .Requires()
            .IsNotNullOrWhiteSpace(Name, nameof(Name), $"Campo {nameof(Name)} não foi informado")
            .IsNotNullOrWhiteSpace(Role, nameof(Role), $"Campo {nameof(Role)} não foi informado")
        );
    }
}