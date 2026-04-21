using HarmoniAPP.Core.Models.Opportunities;

namespace HarmoniAPP.Core.Services;

public sealed class OpportunitiesApiClient : AuthorizedApiClient
{
    public OpportunitiesApiClient(AuthApiClient authApiClient, HttpClient httpClient, ITokenStore tokenStore)
        : base(authApiClient, httpClient, tokenStore)
    {
    }

    public async Task<IReadOnlyList<OpportunityItemResponse>> GetAsync(
        string? search = null,
        string? stage = null,
        CancellationToken cancellationToken = default) =>
        await GetAsync<IReadOnlyList<OpportunityItemResponse>>(
            BuildQueryString(
                "api/v1/oportunidades",
                [
                    new("search", search),
                    new("stage", stage)
                ]),
            cancellationToken) ?? Array.Empty<OpportunityItemResponse>();

    public Task<OpportunityItemResponse> CreateAsync(CreateOpportunityRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateOpportunityRequest, OpportunityItemResponse>("api/v1/oportunidades", request, cancellationToken);
}
