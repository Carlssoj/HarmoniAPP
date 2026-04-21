namespace HarmoniAPP.Core.Services;

public interface ITokenStore
{
    Task<string?> GetAccessTokenAsync();

    Task<string?> GetRefreshTokenAsync();

    Task SetAccessTokenAsync(string accessToken);

    Task SetRefreshTokenAsync(string refreshToken);

    Task SetTokensAsync(string accessToken, string refreshToken);

    Task ClearAsync();
}
