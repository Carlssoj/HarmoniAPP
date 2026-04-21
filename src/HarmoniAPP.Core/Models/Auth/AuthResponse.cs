namespace HarmoniAPP.Core.Models.Auth;

public sealed record AuthResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc,
    string DisplayName,
    string Profile,
    string Team);
