using HarmoniAPP.Core.Models.Tasks;
using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Models;
using HarmoniAPP.Mobile.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HarmoniAPP.Mobile.Pages;

public partial class TasksPage : ContentPage
{
    private readonly TasksApiClient _tasksApiClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyList<FormOption> _statusOptions =
    [
        new(string.Empty, "Todas as tarefas"),
        new("pending", "Pendentes"),
        new("completed", "Concluídas")
    ];
    private readonly IReadOnlyList<FormOption> _priorityOptions =
    [
        new(string.Empty, "Todas as prioridades"),
        new("Low", "Baixa"),
        new("Medium", "Média"),
        new("High", "Alta")
    ];
    private bool _filtersInitialized;
    private bool _isLoading;

    public TasksPage(TasksApiClient tasksApiClient, IServiceProvider serviceProvider)
    {
        _tasksApiClient = tasksApiClient;
        _serviceProvider = serviceProvider;
        InitializeComponent();
        InitializeFilters();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        if (_isLoading)
        {
            return;
        }

        _isLoading = true;

        try
        {
            StatusLabel.IsVisible = false;
            var tasks = await _tasksApiClient.GetAsync(
                NormalizeSearch(TaskSearchBar.Text),
                ResolveCompletedFilter(),
                NormalizeFilterValue(TaskPriorityPicker.SelectedItem),
                CancellationToken.None);
            TasksCollectionView.ItemsSource = tasks;
            PendingTasksValueLabel.Text = tasks.Count(task => !task.Concluida).ToString();
        }
        catch (Exception exception)
        {
            StatusLabel.Text = MobileErrorFormatter.Format(exception);
            StatusLabel.IsVisible = true;
        }
        finally
        {
            TasksRefreshView.IsRefreshing = false;
            _isLoading = false;
        }
    }

    private async void OnRefreshRequested(object? sender, EventArgs e) => await LoadAsync();

    private async void OnCreateTaskClicked(object? sender, EventArgs e)
    {
        var page = _serviceProvider.GetRequiredService<CreateTaskPage>();
        await Navigation.PushModalAsync(new NavigationPage(page));
    }

    private async void OnSearchSubmitted(object? sender, EventArgs e) => await LoadAsync();

    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_filtersInitialized && string.IsNullOrWhiteSpace(e.NewTextValue) && !string.IsNullOrWhiteSpace(e.OldTextValue))
        {
            await LoadAsync();
        }
    }

    private async void OnStatusChanged(object? sender, EventArgs e)
    {
        if (_filtersInitialized)
        {
            await LoadAsync();
        }
    }

    private async void OnPriorityChanged(object? sender, EventArgs e)
    {
        if (_filtersInitialized)
        {
            await LoadAsync();
        }
    }

    private async void OnTaskSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not TaskItemResponse task)
        {
            return;
        }

        TasksCollectionView.SelectedItem = null;
        var page = _serviceProvider.GetRequiredService<EditTaskPage>();
        page.Initialize(task);
        await Navigation.PushModalAsync(new NavigationPage(page));
    }

    private void InitializeFilters()
    {
        TaskStatusPicker.ItemsSource = _statusOptions.ToList();
        TaskStatusPicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        TaskStatusPicker.SelectedItem = _statusOptions[0];

        TaskPriorityPicker.ItemsSource = _priorityOptions.ToList();
        TaskPriorityPicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        TaskPriorityPicker.SelectedItem = _priorityOptions[0];

        _filtersInitialized = true;
    }

    private bool? ResolveCompletedFilter()
    {
        var selectedValue = NormalizeFilterValue(TaskStatusPicker.SelectedItem);
        return selectedValue switch
        {
            "pending" => false,
            "completed" => true,
            _ => null
        };
    }

    private static string? NormalizeSearch(string? searchText) =>
        string.IsNullOrWhiteSpace(searchText) ? null : searchText.Trim();

    private static string? NormalizeFilterValue(object? selectedItem)
    {
        var value = (selectedItem as FormOption)?.Value;
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
