using HarmoniAPP.Core.Models.Tasks;
using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HarmoniAPP.Mobile.Pages;

public partial class TasksPage : ContentPage
{
    private readonly TasksApiClient _tasksApiClient;
    private readonly IServiceProvider _serviceProvider;
    private bool _isLoading;

    public TasksPage(TasksApiClient tasksApiClient, IServiceProvider serviceProvider)
    {
        _tasksApiClient = tasksApiClient;
        _serviceProvider = serviceProvider;
        InitializeComponent();
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
            var tasks = await _tasksApiClient.GetAsync();
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
}
