namespace HarmoniAPP.Core.Models.Customers;

public sealed record CreateCustomerRequest(
    string Empresa,
    string? Segmento,
    string ContatoPrincipal,
    string Email,
    string? Telefone,
    string? Saude,
    decimal? ReceitaMensal,
    string? Responsavel,
    string? Equipe,
    DateTime? UltimaInteracao,
    string? Observacoes);
