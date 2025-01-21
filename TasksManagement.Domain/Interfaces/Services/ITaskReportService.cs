using TasksManagement.API.Models.OutputModels.Task;

namespace TasksManagement.Domain.Interfaces.Services;
/// <summary>
/// Serviço para geração de relatórios relacionados a tarefas.
/// </summary>
public interface ITaskReportService
{
    /// <summary>
    /// Gera um relatório de tarefas concluídas por um usuário nos últimos X dias.
    /// Se não for informado, o padrão será 30 dias.
    /// </summary>
    /// <param name="userId">ID do usuário para o qual gerar o relatório.</param>
    /// <param name="days">Quantidade de dias para o filtro (padrão: 30).</param>
    /// <returns>Relatório de tarefas concluídas por usuário.</returns>
    Task<Result<IEnumerable<TaskReportModel>>> GenerateCompletedTasksByUserReportAsync(Guid userId, int days = 30);

    /// <summary>
    /// Gera um relatório com as top 10 tarefas com mais comentários nos últimos X dias.
    /// Se não for informado, o padrão será 30 dias.
    /// </summary>
    /// <param name="days">Quantidade de dias para o filtro (padrão: 30).</param>
    /// <returns>Relatório de tarefas com mais comentários.</returns>
    Task<Result<IEnumerable<TaskReportModel>>> GenerateTopTasksByCommentsAsync(int days = 30);
}