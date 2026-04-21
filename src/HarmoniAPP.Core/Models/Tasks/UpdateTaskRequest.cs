namespace HarmoniAPP.Core.Models.Tasks;

public sealed record UpdateTaskRequest(
    string Titulo,
    string RelacionadoA,
    string? Responsavel,
    string? Equipe,
    string Prioridade,
    DateTime? DataVencimento,
    string? Observacoes,
    bool Concluida);
