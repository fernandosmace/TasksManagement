using FluentAssertions;
using Flunt.Notifications;
using TasksManagement.Domain;

namespace TasksManagement.Tests.Entities;
public class ResultTests
{
    [Fact]
    public void Result_Should_Be_Success_With_Message_And_Data()
    {
        // Arrange
        var data = new { Id = 1, Name = "Test" };
        var message = "Operação concluída com sucesso.";

        // Act
        var result = Result.Success(message, data);

        // Assert
        result.Message.Should().Be(message, "A mensagem deve ser a fornecida");
        result.Data.Should().BeEquivalentTo(data, "Os dados devem corresponder aos dados fornecidos");
        result.StatusCode.Should().BeNull("O código de status deve ser nulo em sucesso genérico");
        result.IsValid.Should().BeTrue("O resultado deve ser válido (sucesso)");
    }

    [Fact]
    public void Result_Should_Be_Failure_With_Message_And_Notifications()
    {
        // Arrange
        var message = "Operação falhou.";
        var notifications = new List<Notification>
            {
                new Notification("Field1", "Campo Field1 é inválido."),
                new Notification("Field2", "Campo Field2 não pode estar vazio.")
            };

        // Act
        var result = Result.Failure(message, notifications);

        // Assert
        result.IsValid.Should().BeFalse("O resultado deve indicar falha");
        result.Message.Should().Be(message, "A mensagem deve ser a fornecida");
        result.Notifications.Should().BeEquivalentTo(notifications, "As notificações devem corresponder às fornecidas");
        result.StatusCode.Should().BeNull("O código de status deve ser nulo se não explicitamente definido");
    }

    [Fact]
    public void Result_Generic_Should_Be_Success_With_Data_And_Message()
    {
        // Arrange
        var data = new { Id = 42, Description = "Sample Data" };
        var message = "Operação genérica concluída com sucesso.";

        // Act
        var result = Result.Success(data, message);

        // Assert
        result.Message.Should().Be(message, "A mensagem deve ser a fornecida");
        result.Data.Should().BeEquivalentTo(data, "Os dados devem corresponder aos fornecidos");
        result.StatusCode.Should().BeNull("O código de status deve ser nulo em sucesso genérico");
        result.IsValid.Should().BeTrue("O resultado deve ser válido (sucesso)");
    }

    [Fact]
    public void Result_Generic_Should_Be_Failure_With_Message_And_Notifications()
    {
        // Arrange
        var message = "Operação genérica falhou.";
        var notifications = new List<Notification>
            {
                new Notification("FieldA", "Campo FieldA é obrigatório."),
                new Notification("FieldB", "Campo FieldB possui valor inválido.")
            };

        // Act
        var result = Result.Failure<object>(message, notifications);

        // Assert
        result.IsValid.Should().BeFalse("O resultado deve indicar falha");
        result.Message.Should().Be(message, "A mensagem deve ser a fornecida");
        result.Data.Should().BeNull("Os dados devem ser nulos para um resultado de falha");
        result.Notifications.Should().BeEquivalentTo(notifications, "As notificações devem corresponder às fornecidas");
        result.StatusCode.Should().BeNull("O código de status deve ser nulo se não explicitamente definido");
    }

    [Fact]
    public void Result_Should_Be_Failure_With_Empty_Notifications()
    {
        // Arrange
        var message = "Operação falhou sem erros específicos.";

        // Act
        var result = Result.Failure(message, notifications: new List<Notification> { new Notification(null, "Falha") });

        // Assert
        result.IsValid.Should().BeFalse("O resultado deve indicar falha");
        result.Message.Should().Be(message, "A mensagem deve ser a fornecida");
        result.StatusCode.Should().BeNull("O código de status deve ser nulo se não explicitamente definido");
    }

    [Fact]
    public void Result_Generic_Should_Have_Data_Empty_When_Failure_With_Empty_Data()
    {
        // Arrange
        var message = "Falha ao processar dados.";

        // Act
        var result = Result.Failure<object>(message);

        // Assert
        result.IsValid.Should().BeFalse("O resultado deve indicar falha");
        result.Message.Should().Be(message, "A mensagem deve ser a fornecida");
        result.Data.Should().BeNull("Os dados devem ser nulos em caso de falha");
        result.StatusCode.Should().BeNull("O código de status deve ser nulo em caso de falha");
    }
}