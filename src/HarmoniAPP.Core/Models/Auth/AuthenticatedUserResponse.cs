namespace HarmoniAPP.Core.Models.Auth;

public sealed record AuthenticatedUserResponse(
    string UserId,
    string DisplayName,
    string Profile,
    string Team,
    bool HasCrossTeamVisibility);
