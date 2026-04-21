using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HarmoniAPP.Core.Services;

public abstract class AuthorizedApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;

    protected AuthorizedApiClient(HttpClient httpClient, ITokenStore tokenStore)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
    }

    protected async Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        await ApplyTokenAsync();
        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }

    protected async Task<TResponse> PostAsync<TRequest, TResponse>(
        string requestUri,
        TRequest payload,
        CancellationToken cancellationToken = default)
    {
        await ApplyTokenAsync();
        using var response = await _httpClient.PostAsJsonAsync(requestUri, payload, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("A API retornou uma resposta vazia.");
    }

    private async Task ApplyTokenAsync()
    {
        var token = await _tokenStore.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Nenhuma sessão ativa foi encontrada.");
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        var message = string.IsNullOrWhiteSpace(payload)
            ? $"A API retornou {(int)response.StatusCode}."
            : payload;

        throw new HttpRequestException(message, null, response.StatusCode);
    }
}
