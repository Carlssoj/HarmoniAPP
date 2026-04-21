using HarmoniAPP.Core.Models.Interactions;

namespace HarmoniAPP.Core.Services;

public sealed class InteractionsApiClient : AuthorizedApiClient
{
    public InteractionsApiClient(AuthApiClient authApiClient, HttpClient httpClient, ITokenStore tokenStore)
        : base(authApiClient, httpClient, tokenStore)
    {
    }

    public async Task<IReadOnlyList<InteractionItemResponse>> GetAsync(
        string? search = null,
        string? type = null,
        CancellationToken cancellationToken = default) =>
        await GetAsync<IReadOnlyList<InteractionItemResponse>>(
            BuildQueryString(
                "api/v1/interacoes",
                [
                    new("search", search),
                    new("type", type)
                ]),
            cancellationToken) ?? Array.Empty<InteractionItemResponse>();

    public Task<InteractionItemResponse> CreateAsync(CreateInteractionRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateInteractionRequest, InteractionItemResponse>("api/v1/interacoes", request, cancellationToken);
}
