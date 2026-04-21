using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Services;

namespace HarmoniAPP.Mobile.Pages;

public partial class PipelinePage : ContentPage
{
    private readonly LeadsApiClient _leadsApiClient;
    private readonly CustomersApiClient _customersApiClient;
    private readonly OpportunitiesApiClient _opportunitiesApiClient;
    private PipelineSection _selectedSection = PipelineSection.Leads;
    private bool _isLoading;

    public PipelinePage(
        LeadsApiClient leadsApiClient,
        CustomersApiClient customersApiClient,
        OpportunitiesApiClient opportunitiesApiClient)
    {
        _leadsApiClient = leadsApiClient;
        _customersApiClient = customersApiClient;
        _opportunitiesApiClient = opportunitiesApiClient;
        InitializeComponent();
        ApplySectionState();
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

            switch (_selectedSection)
            {
                case PipelineSection.Leads:
                    var leads = await _leadsApiClient.GetAsync();
                    LeadsCollectionView.ItemsSource = leads;
                    PipelineHintLabel.Text = $"{leads.Count} lead(s) carregados para o time atual.";
                    break;

                case PipelineSection.Customers:
                    var customers = await _customersApiClient.GetAsync();
                    CustomersCollectionView.ItemsSource = customers;
                    PipelineHintLabel.Text = $"{customers.Count} cliente(s) ativos na sua carteira.";
                    break;

                case PipelineSection.Opportunities:
                    var opportunities = await _opportunitiesApiClient.GetAsync();
                    OpportunitiesCollectionView.ItemsSource = opportunities;
                    PipelineHintLabel.Text = $"{opportunities.Count} oportunidade(s) abertas neste momento.";
                    break;
            }
        }
        catch (Exception exception)
        {
            StatusLabel.Text = MobileErrorFormatter.Format(exception);
            StatusLabel.IsVisible = true;
        }
        finally
        {
            PipelineRefreshView.IsRefreshing = false;
            _isLoading = false;
        }
    }

    private void ApplySectionState()
    {
        LeadsCollectionView.IsVisible = _selectedSection == PipelineSection.Leads;
        CustomersCollectionView.IsVisible = _selectedSection == PipelineSection.Customers;
        OpportunitiesCollectionView.IsVisible = _selectedSection == PipelineSection.Opportunities;

        ApplySegmentStyle(LeadsSegmentButton, _selectedSection == PipelineSection.Leads);
        ApplySegmentStyle(CustomersSegmentButton, _selectedSection == PipelineSection.Customers);
        ApplySegmentStyle(OpportunitiesSegmentButton, _selectedSection == PipelineSection.Opportunities);
    }

    private static void ApplySegmentStyle(Button button, bool isActive)
    {
        button.BackgroundColor = isActive
            ? (Color)Application.Current!.Resources["BrandPrimary"]
            : (Color)Application.Current!.Resources["SurfaceSoft"];
        button.TextColor = isActive
            ? Colors.White
            : (Color)Application.Current!.Resources["TextPrimary"];
    }

    private async Task ChangeSectionAsync(PipelineSection section)
    {
        _selectedSection = section;
        ApplySectionState();
        await LoadAsync();
    }

    private async void OnShowLeadsClicked(object? sender, EventArgs e) => await ChangeSectionAsync(PipelineSection.Leads);

    private async void OnShowCustomersClicked(object? sender, EventArgs e) => await ChangeSectionAsync(PipelineSection.Customers);

    private async void OnShowOpportunitiesClicked(object? sender, EventArgs e) => await ChangeSectionAsync(PipelineSection.Opportunities);

    private async void OnRefreshRequested(object? sender, EventArgs e) => await LoadAsync();

    private enum PipelineSection
    {
        Leads,
        Customers,
        Opportunities
    }
}
