using HarmoniAPP.Core.Models.Tasks;

namespace HarmoniAPP.Core.Services;

public sealed class TasksApiClient : AuthorizedApiClient
{
    public TasksApiClient(HttpClient httpClient, ITokenStore tokenStore)
        : base(httpClient, tokenStore)
    {
    }

    public async Task<IReadOnlyList<TaskItemResponse>> GetAsync(CancellationToken cancellationToken = default) =>
        await GetAsync<IReadOnlyList<TaskItemResponse>>("api/v1/tarefas", cancellationToken) ?? Array.Empty<TaskItemResponse>();

    public Task<TaskItemResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateTaskRequest, TaskItemResponse>("api/v1/tarefas", request, cancellationToken);
}
