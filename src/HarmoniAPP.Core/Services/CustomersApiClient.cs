using HarmoniAPP.Core.Models.Customers;

namespace HarmoniAPP.Core.Services;

public sealed class CustomersApiClient : AuthorizedApiClient
{
    public CustomersApiClient(HttpClient httpClient, ITokenStore tokenStore)
        : base(httpClient, tokenStore)
    {
    }

    public async Task<IReadOnlyList<CustomerItemResponse>> GetAsync(CancellationToken cancellationToken = default) =>
        await GetAsync<IReadOnlyList<CustomerItemResponse>>("api/v1/clientes", cancellationToken) ?? Array.Empty<CustomerItemResponse>();
}
