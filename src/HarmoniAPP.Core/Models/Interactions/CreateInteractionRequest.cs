namespace HarmoniAPP.Core.Models.Interactions;

public sealed record CreateInteractionRequest(
    string Assunto,
    string Tipo,
    string TipoRegistro,
    string RelacionadoA,
    string? Responsavel,
    string? Equipe,
    DateTime? OcorreuEm,
    string Resumo,
    string? ProximaAcao,
    DateTime? DataProximaAcao);
