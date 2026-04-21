namespace HarmoniAPP.Core.Models.Opportunities;

public sealed record OpportunityItemResponse(
    Guid Id,
    string Titulo,
    string Conta,
    string Etapa,
    decimal Valor,
    int Probabilidade,
    DateTime DataFechamentoEsperada,
    string Responsavel,
    string Equipe,
    bool Estrategica);
