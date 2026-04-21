using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HarmoniAPP.Mobile.Pages;

public partial class InteractionsPage : ContentPage
{
    private readonly InteractionsApiClient _interactionsApiClient;
    private readonly IServiceProvider _serviceProvider;
    private bool _isLoading;

    public InteractionsPage(InteractionsApiClient interactionsApiClient, IServiceProvider serviceProvider)
    {
        _interactionsApiClient = interactionsApiClient;
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
            var interactions = await _interactionsApiClient.GetAsync();
            InteractionsCollectionView.ItemsSource = interactions;
            RecentInteractionsValueLabel.Text = interactions.Count.ToString();
        }
        catch (Exception exception)
        {
            StatusLabel.Text = MobileErrorFormatter.Format(exception);
            StatusLabel.IsVisible = true;
        }
        finally
        {
            InteractionsRefreshView.IsRefreshing = false;
            _isLoading = false;
        }
    }

    private async void OnRefreshRequested(object? sender, EventArgs e) => await LoadAsync();

    private async void OnCreateInteractionClicked(object? sender, EventArgs e)
    {
        var page = _serviceProvider.GetRequiredService<CreateInteractionPage>();
        await Navigation.PushModalAsync(new NavigationPage(page));
    }
}
