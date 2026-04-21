using HarmoniAPP.Core.Models.Opportunities;

namespace HarmoniAPP.Core.Services;

public sealed class OpportunitiesApiClient : AuthorizedApiClient
{
    public OpportunitiesApiClient(HttpClient httpClient, ITokenStore tokenStore)
        : base(httpClient, tokenStore)
    {
    }

    public async Task<IReadOnlyList<OpportunityItemResponse>> GetAsync(CancellationToken cancellationToken = default) =>
        await GetAsync<IReadOnlyList<OpportunityItemResponse>>("api/v1/oportunidades", cancellationToken) ?? Array.Empty<OpportunityItemResponse>();
}
