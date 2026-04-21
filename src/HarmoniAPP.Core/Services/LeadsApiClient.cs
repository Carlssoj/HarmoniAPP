using HarmoniAPP.Core.Models.Leads;

namespace HarmoniAPP.Core.Services;

public sealed class LeadsApiClient : AuthorizedApiClient
{
    public LeadsApiClient(HttpClient httpClient, ITokenStore tokenStore)
        : base(httpClient, tokenStore)
    {
    }

    public async Task<IReadOnlyList<LeadItemResponse>> GetAsync(CancellationToken cancellationToken = default) =>
        await GetAsync<IReadOnlyList<LeadItemResponse>>("api/v1/leads", cancellationToken) ?? Array.Empty<LeadItemResponse>();
}
