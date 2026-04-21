using HarmoniAPP.Core.Models.Tasks;

namespace HarmoniAPP.Core.Services;

public sealed class TasksApiClient : AuthorizedApiClient
{
    public TasksApiClient(AuthApiClient authApiClient, HttpClient httpClient, ITokenStore tokenStore)
        : base(authApiClient, httpClient, tokenStore)
    {
    }

    public async Task<IReadOnlyList<TaskItemResponse>> GetAsync(
        string? search = null,
        bool? completed = null,
        string? priority = null,
        CancellationToken cancellationToken = default) =>
        await GetAsync<IReadOnlyList<TaskItemResponse>>(
            BuildQueryString(
                "api/v1/tarefas",
                [
                    new("search", search),
                    new("completed", completed?.ToString()?.ToLowerInvariant()),
                    new("priority", priority)
                ]),
            cancellationToken) ?? Array.Empty<TaskItemResponse>();

    public Task<TaskItemResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateTaskRequest, TaskItemResponse>("api/v1/tarefas", request, cancellationToken);

    public Task<TaskItemResponse> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateTaskRequest, TaskItemResponse>($"api/v1/tarefas/{id}", request, cancellationToken);
}
