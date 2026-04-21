using System.Net.Http.Headers;
using System.Net.Http.Json;
using HarmoniAPP.Core.Models.Auth;

namespace HarmoniAPP.Core.Services;

public sealed class AuthApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public AuthApiClient(HttpClient httpClient, ITokenStore tokenStore)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
    }

    public async Task<AuthResponse> LoginAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/v1/auth/login",
            new { Login = login, Password = password },
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("A API não retornou o token de autenticação.");

        await _tokenStore.SetTokensAsync(payload.AccessToken, payload.RefreshToken);
        return payload;
    }

    public async Task<AuthenticatedUserResponse?> GetMeAsync(CancellationToken cancellationToken = default)
    {
        var token = await _tokenStore.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/auth/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshAsync(cancellationToken);
                if (!refreshed)
                {
                    return null;
                }

                var refreshedToken = await _tokenStore.GetAccessTokenAsync();
                using var retryRequest = new HttpRequestMessage(HttpMethod.Get, "api/v1/auth/me");
                retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshedToken);
                using var retryResponse = await _httpClient.SendAsync(retryRequest, cancellationToken);
                if (retryResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await _tokenStore.ClearAsync();
                    return null;
                }

                retryResponse.EnsureSuccessStatusCode();
                return await retryResponse.Content.ReadFromJsonAsync<AuthenticatedUserResponse>(cancellationToken: cancellationToken);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuthenticatedUserResponse>(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException httpRequestException) when (httpRequestException.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _tokenStore.ClearAsync();
            return null;
        }
    }

    public async Task<bool> TryRefreshAsync(CancellationToken cancellationToken = default)
    {
        await _refreshLock.WaitAsync(cancellationToken);

        try
        {
            var refreshToken = await _tokenStore.GetRefreshTokenAsync();
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                await _tokenStore.ClearAsync();
                return false;
            }

            using var response = await _httpClient.PostAsJsonAsync(
                "api/v1/auth/refresh",
                new RefreshTokenRequest(refreshToken),
                cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await _tokenStore.ClearAsync();
                return false;
            }

            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException("A API não retornou a renovação da sessão.");

            await _tokenStore.SetTokensAsync(payload.AccessToken, payload.RefreshToken);
            return true;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await _tokenStore.GetAccessTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/auth/logout");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                using var response = await _httpClient.SendAsync(request, cancellationToken);
            }
        }
        catch
        {
            // Logout local continua mesmo se a revogação remota falhar.
        }
        finally
        {
            await _tokenStore.ClearAsync();
        }
    }
}
