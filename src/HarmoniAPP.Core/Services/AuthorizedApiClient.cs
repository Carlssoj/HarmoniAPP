using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace HarmoniAPP.Core.Services;

public abstract class AuthorizedApiClient
{
    private readonly AuthApiClient _authApiClient;
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;

    protected AuthorizedApiClient(AuthApiClient authApiClient, HttpClient httpClient, ITokenStore tokenStore)
    {
        _authApiClient = authApiClient;
        _httpClient = httpClient;
        _tokenStore = tokenStore;
    }

    protected async Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(
            () => new HttpRequestMessage(HttpMethod.Get, requestUri),
            cancellationToken);
    }

    protected async Task<TResponse> PostAsync<TRequest, TResponse>(
        string requestUri,
        TRequest payload,
        CancellationToken cancellationToken = default)
    {
        return await SendAsync<TResponse>(
            () => CreateJsonRequest(HttpMethod.Post, requestUri, payload),
            cancellationToken)
            ?? throw new InvalidOperationException("A API retornou uma resposta vazia.");
    }

    protected async Task<TResponse> PutAsync<TRequest, TResponse>(
        string requestUri,
        TRequest payload,
        CancellationToken cancellationToken = default)
    {
        return await SendAsync<TResponse>(
            () => CreateJsonRequest(HttpMethod.Put, requestUri, payload),
            cancellationToken)
            ?? throw new InvalidOperationException("A API retornou uma resposta vazia.");
    }

    protected static string BuildQueryString(string requestUri, IEnumerable<KeyValuePair<string, string?>> queryParameters)
    {
        var encodedQuery = queryParameters
            .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Value))
            .Select(parameter =>
                $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value!)}")
            .ToList();

        return encodedQuery.Count == 0
            ? requestUri
            : $"{requestUri}?{string.Join("&", encodedQuery)}";
    }

    private async Task<T?> SendAsync<T>(Func<HttpRequestMessage> requestFactory, CancellationToken cancellationToken)
    {
        var token = await _tokenStore.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Nenhuma sessão ativa foi encontrada.");
        }

        using var request = requestFactory();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized && await _authApiClient.TryRefreshAsync(cancellationToken))
        {
            var refreshedToken = await _tokenStore.GetAccessTokenAsync();
            if (string.IsNullOrWhiteSpace(refreshedToken))
            {
                throw new InvalidOperationException("Não foi possível renovar a sessão do aplicativo.");
            }

            using var retryRequest = requestFactory();
            retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshedToken);
            using var retryResponse = await _httpClient.SendAsync(retryRequest, cancellationToken);
            await EnsureSuccessAsync(retryResponse, cancellationToken);
            return await retryResponse.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }

        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }

    private static HttpRequestMessage CreateJsonRequest<TRequest>(HttpMethod method, string requestUri, TRequest payload)
    {
        var request = new HttpRequestMessage(method, requestUri)
        {
            Content = JsonContent.Create(payload)
        };

        request.Content.Headers.ContentType!.CharSet = Encoding.UTF8.WebName;
        return request;
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
