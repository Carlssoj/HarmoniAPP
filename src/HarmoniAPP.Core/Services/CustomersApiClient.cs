using HarmoniAPP.Core.Models.Customers;

namespace HarmoniAPP.Core.Services;

public sealed class CustomersApiClient : AuthorizedApiClient
{
    public CustomersApiClient(AuthApiClient authApiClient, HttpClient httpClient, ITokenStore tokenStore)
        : base(authApiClient, httpClient, tokenStore)
    {
    }

    public async Task<IReadOnlyList<CustomerItemResponse>> GetAsync(
        string? search = null,
        string? health = null,
        CancellationToken cancellationToken = default) =>
        await GetAsync<IReadOnlyList<CustomerItemResponse>>(
            BuildQueryString(
                "api/v1/clientes",
                [
                    new("search", search),
                    new("health", health)
                ]),
            cancellationToken) ?? Array.Empty<CustomerItemResponse>();

    public Task<CustomerItemResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateCustomerRequest, CustomerItemResponse>("api/v1/clientes", request, cancellationToken);
}
