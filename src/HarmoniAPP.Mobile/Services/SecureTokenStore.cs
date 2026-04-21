using HarmoniAPP.Core.Services;

namespace HarmoniAPP.Mobile.Services;

public sealed class SecureTokenStore : ITokenStore
{
    private const string AccessTokenKey = "harmoni_access_token";

    public Task<string?> GetAccessTokenAsync() => SecureStorage.Default.GetAsync(AccessTokenKey);

    public Task SetAccessTokenAsync(string accessToken) => SecureStorage.Default.SetAsync(AccessTokenKey, accessToken);

    public Task ClearAsync()
    {
        SecureStorage.Default.Remove(AccessTokenKey);
        return Task.CompletedTask;
    }
}
