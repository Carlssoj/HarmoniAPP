namespace HarmoniAPP.Core.Models.Dashboard;

public sealed record DashboardSummaryResponse(
    int Leads,
    int Clientes,
    int OportunidadesAbertas,
    int TarefasPendentes,
    decimal PipelineAberto,
    decimal ReceitaMensal,
    int InteracoesUltimos7Dias);
