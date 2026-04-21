namespace HarmoniAPP.Core.Models.Leads;

public sealed record CreateLeadRequest(
    string Nome,
    string Empresa,
    string Email,
    string? Telefone,
    string? Status,
    decimal? ValorPotencial,
    string? Origem,
    string? Responsavel,
    string? Equipe,
    DateTime? ProximoFollowUp,
    string? Observacoes);
