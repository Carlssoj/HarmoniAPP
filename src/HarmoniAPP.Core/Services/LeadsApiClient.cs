using HarmoniAPP.Core.Models.Leads;

namespace HarmoniAPP.Core.Services;

public sealed class LeadsApiClient : AuthorizedApiClient
{
    public LeadsApiClient(AuthApiClient authApiClient, HttpClient httpClient, ITokenStore tokenStore)
        : base(authApiClient, httpClient, tokenStore)
    {
    }

    public async Task<IReadOnlyList<LeadItemResponse>> GetAsync(
        string? search = null,
        string? status = null,
        CancellationToken cancellationToken = default) =>
        await GetAsync<IReadOnlyList<LeadItemResponse>>(
            BuildQueryString(
                "api/v1/leads",
                [
                    new("search", search),
                    new("status", status)
                ]),
            cancellationToken) ?? Array.Empty<LeadItemResponse>();

    public Task<LeadItemResponse> CreateAsync(CreateLeadRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateLeadRequest, LeadItemResponse>("api/v1/leads", request, cancellationToken);
}
