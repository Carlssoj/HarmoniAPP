namespace HarmoniAPP.Core.Models.Customers;

public sealed record CustomerItemResponse(
    Guid Id,
    string Empresa,
    string Segmento,
    string ContatoPrincipal,
    string Email,
    string Telefone,
    string Saude,
    decimal ReceitaMensal,
    string Responsavel,
    string Equipe,
    DateTime UltimaInteracao);
