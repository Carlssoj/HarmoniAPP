using HarmoniAPP.Core.Models.Interactions;

namespace HarmoniAPP.Core.Services;

public sealed class InteractionsApiClient : AuthorizedApiClient
{
    public InteractionsApiClient(HttpClient httpClient, ITokenStore tokenStore)
        : base(httpClient, tokenStore)
    {
    }

    public async Task<IReadOnlyList<InteractionItemResponse>> GetAsync(CancellationToken cancellationToken = default) =>
        await GetAsync<IReadOnlyList<InteractionItemResponse>>("api/v1/interacoes", cancellationToken) ?? Array.Empty<InteractionItemResponse>();

    public Task<InteractionItemResponse> CreateAsync(CreateInteractionRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateInteractionRequest, InteractionItemResponse>("api/v1/interacoes", request, cancellationToken);
}
