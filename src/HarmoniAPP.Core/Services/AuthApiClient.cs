using System.Net.Http.Headers;
using System.Net.Http.Json;
using HarmoniAPP.Core.Models.Auth;

namespace HarmoniAPP.Core.Services;

public sealed class AuthApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;

    public AuthApiClient(HttpClient httpClient, ITokenStore tokenStore)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
    }

    public async Task<AuthResponse> LoginAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/v1/auth/login",
            new { Login = login, Password = password },
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("A API não retornou o token de autenticação.");

        await _tokenStore.SetAccessTokenAsync(payload.AccessToken);
        return payload;
    }

    public async Task<AuthenticatedUserResponse?> GetMeAsync(CancellationToken cancellationToken = default)
    {
        var token = await _tokenStore.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _httpClient.GetFromJsonAsync<AuthenticatedUserResponse>("api/v1/auth/me", cancellationToken);
    }

    public Task LogoutAsync() => _tokenStore.ClearAsync();
}
