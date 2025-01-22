using TasksManagement.Domain.Entities;

namespace TasksManagement.Tests.Mocks.Entities;
public static class UserMock
{
    public static User GetDefault() => new User("Usuário Padrão", "Gerente");

    public static User GetWithEmptyName() => new User("", "User");

    public static User GetWithNullName() => new User(null, "User");
    public static User GetWithWhiteSpaceName() => new User(" ", "User");

    public static User GetWithEmptyRole() => new User("Usuário sem papel", "");
    public static User GetWithNullRole() => new User("Usuário sem papel", null);
    public static User GetWithWhiteSpaceRole() => new User("Usuário sem papel", " ");
}
