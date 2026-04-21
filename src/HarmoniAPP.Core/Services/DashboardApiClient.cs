using System.Net.Http.Headers;
using System.Net.Http.Json;
using HarmoniAPP.Core.Models.Dashboard;

namespace HarmoniAPP.Core.Services;

public sealed class DashboardApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;

    public DashboardApiClient(HttpClient httpClient, ITokenStore tokenStore)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
    }

    public async Task<DashboardSummaryResponse?> GetResumoAsync(CancellationToken cancellationToken = default)
    {
        var token = await _tokenStore.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _httpClient.GetFromJsonAsync<DashboardSummaryResponse>("api/v1/dashboard/resumo", cancellationToken);
    }
}
