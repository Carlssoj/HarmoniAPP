using HarmoniAPP.Core.Models.Dashboard;

namespace HarmoniAPP.Core.Services;

public sealed class DashboardApiClient : AuthorizedApiClient
{
    public DashboardApiClient(HttpClient httpClient, ITokenStore tokenStore)
        : base(httpClient, tokenStore)
    {
    }

    public async Task<DashboardSummaryResponse?> GetResumoAsync(CancellationToken cancellationToken = default)
        => await GetAsync<DashboardSummaryResponse>("api/v1/dashboard/resumo", cancellationToken);
}
