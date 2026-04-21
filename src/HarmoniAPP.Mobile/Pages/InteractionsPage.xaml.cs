using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Models;
using HarmoniAPP.Mobile.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HarmoniAPP.Mobile.Pages;

public partial class InteractionsPage : ContentPage
{
    private readonly InteractionsApiClient _interactionsApiClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyList<FormOption> _typeOptions =
    [
        new(string.Empty, "Todos os tipos"),
        new("Call", "Ligação"),
        new("Meeting", "Reunião"),
        new("Email", "E-mail"),
        new("Proposal", "Proposta"),
        new("QuarterlyBusinessReview", "QBR"),
        new("Onboarding", "Onboarding")
    ];
    private bool _filtersInitialized;
    private bool _isLoading;

    public InteractionsPage(InteractionsApiClient interactionsApiClient, IServiceProvider serviceProvider)
    {
        _interactionsApiClient = interactionsApiClient;
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
            var interactions = await _interactionsApiClient.GetAsync(
                NormalizeSearch(InteractionSearchBar.Text),
                NormalizeFilterValue(InteractionTypePicker.SelectedItem),
                CancellationToken.None);
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

    private async void OnSearchSubmitted(object? sender, EventArgs e) => await LoadAsync();

    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_filtersInitialized && string.IsNullOrWhiteSpace(e.NewTextValue) && !string.IsNullOrWhiteSpace(e.OldTextValue))
        {
            await LoadAsync();
        }
    }

    private async void OnTypeChanged(object? sender, EventArgs e)
    {
        if (_filtersInitialized)
        {
            await LoadAsync();
        }
    }

    private void InitializeFilters()
    {
        InteractionTypePicker.ItemsSource = _typeOptions.ToList();
        InteractionTypePicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        InteractionTypePicker.SelectedItem = _typeOptions[0];
        _filtersInitialized = true;
    }

    private static string? NormalizeSearch(string? searchText) =>
        string.IsNullOrWhiteSpace(searchText) ? null : searchText.Trim();

    private static string? NormalizeFilterValue(object? selectedItem)
    {
        var value = (selectedItem as FormOption)?.Value;
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
