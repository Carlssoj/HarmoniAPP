namespace HarmoniAPP.Core.Models.Tasks;

public sealed record TaskItemResponse(
    Guid Id,
    string Titulo,
    string RelacionadoA,
    string Responsavel,
    string Equipe,
    string Prioridade,
    DateTime DataVencimento,
    bool Concluida,
    string Observacoes);
