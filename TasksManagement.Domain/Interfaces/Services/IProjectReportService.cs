using TasksManagement.Domain.Models.OutputModels.Project;

namespace TasksManagement.Domain.Interfaces.Services;
/// <summary>
/// Serviço para geração de relatórios relacionados a projetos.
/// </summary>
public interface IProjectReportService
{
    /// <summary>
    /// Gera um relatório dos top 10 projetos com mais tarefas concluídas nos últimos X dias.
    /// Se não for informado, o padrão será 30 dias.
    /// </summary>
    /// <param name="days">Quantidade de dias para o filtro (padrão: 30).</param>
    /// <returns>Relatório de projetos com mais tarefas concluídas.</returns>
    Task<Result<IEnumerable<ProjectReportModel>>> GenerateTopProjectsByCompletedTasksAsync(int days = 30);
}