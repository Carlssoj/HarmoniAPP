namespace HarmoniAPP.Core.Models.Opportunities;

public sealed record CreateOpportunityRequest(
    string Titulo,
    string Conta,
    string? Etapa,
    decimal? Valor,
    int? Probabilidade,
    DateTime? DataFechamentoEsperada,
    string? Responsavel,
    string? Equipe,
    string? Resumo,
    bool? Estrategica);
