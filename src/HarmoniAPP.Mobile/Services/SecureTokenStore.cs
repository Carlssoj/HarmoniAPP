using HarmoniAPP.Core.Services;

namespace HarmoniAPP.Mobile.Services;

public sealed class SecureTokenStore : ITokenStore
{
    private const string AccessTokenKey = "harmoni_access_token";
    private const string RefreshTokenKey = "harmoni_refresh_token";

    public Task<string?> GetAccessTokenAsync() => SecureStorage.Default.GetAsync(AccessTokenKey);

    public Task<string?> GetRefreshTokenAsync() => SecureStorage.Default.GetAsync(RefreshTokenKey);

    public Task SetAccessTokenAsync(string accessToken) => SecureStorage.Default.SetAsync(AccessTokenKey, accessToken);

    public Task SetRefreshTokenAsync(string refreshToken) => SecureStorage.Default.SetAsync(RefreshTokenKey, refreshToken);

    public async Task SetTokensAsync(string accessToken, string refreshToken)
    {
        await SetAccessTokenAsync(accessToken);
        await SetRefreshTokenAsync(refreshToken);
    }

    public Task ClearAsync()
    {
        SecureStorage.Default.Remove(AccessTokenKey);
        SecureStorage.Default.Remove(RefreshTokenKey);
        return Task.CompletedTask;
    }
}
