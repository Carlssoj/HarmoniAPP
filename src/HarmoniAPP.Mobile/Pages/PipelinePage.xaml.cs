using HarmoniAPP.Core.Models.Leads;
using HarmoniAPP.Core.Models.Customers;
using HarmoniAPP.Core.Models.Opportunities;
using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Models;
using HarmoniAPP.Mobile.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HarmoniAPP.Mobile.Pages;

public partial class PipelinePage : ContentPage
{
    private readonly LeadsApiClient _leadsApiClient;
    private readonly CustomersApiClient _customersApiClient;
    private readonly OpportunitiesApiClient _opportunitiesApiClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyList<FormOption> _leadStatusOptions =
    [
        new(string.Empty, "Todos os statuses"),
        new("New", "Novo"),
        new("Contacted", "Contato inicial"),
        new("Qualified", "Qualificado"),
        new("ProposalSent", "Proposta enviada"),
        new("Won", "Ganho"),
        new("Lost", "Perdido")
    ];
    private readonly IReadOnlyList<FormOption> _customerHealthOptions =
    [
        new(string.Empty, "Todas as saúdes"),
        new("Strong", "Saudável"),
        new("Stable", "Monitorar"),
        new("Risk", "Em risco")
    ];
    private readonly IReadOnlyList<FormOption> _opportunityStageOptions =
    [
        new(string.Empty, "Todas as etapas"),
        new("Discovery", "Diagnóstico"),
        new("SolutionDesign", "Desenho da solução"),
        new("Proposal", "Proposta"),
        new("Negotiation", "Negociação"),
        new("ClosedWon", "Ganho"),
        new("ClosedLost", "Perdido")
    ];
    private PipelineSection _selectedSection = PipelineSection.Leads;
    private bool _filtersInitialized;
    private bool _isLoading;

    public PipelinePage(
        LeadsApiClient leadsApiClient,
        CustomersApiClient customersApiClient,
        OpportunitiesApiClient opportunitiesApiClient,
        IServiceProvider serviceProvider)
    {
        _leadsApiClient = leadsApiClient;
        _customersApiClient = customersApiClient;
        _opportunitiesApiClient = opportunitiesApiClient;
        _serviceProvider = serviceProvider;
        InitializeComponent();
        InitializeFilters();
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
            var search = NormalizeSearch(PipelineSearchBar.Text);

            switch (_selectedSection)
            {
                case PipelineSection.Leads:
                    var leads = await _leadsApiClient.GetAsync(
                        search,
                        NormalizeFilterValue(LeadStatusPicker.SelectedItem),
                        CancellationToken.None);
                    LeadsCollectionView.ItemsSource = leads;
                    PipelineHintLabel.Text = $"{leads.Count} lead(s) carregados para o time atual.";
                    break;

                case PipelineSection.Customers:
                    var customers = await _customersApiClient.GetAsync(
                        search,
                        NormalizeFilterValue(CustomerHealthPicker.SelectedItem),
                        CancellationToken.None);
                    CustomersCollectionView.ItemsSource = customers;
                    PipelineHintLabel.Text = $"{customers.Count} cliente(s) ativos na sua carteira.";
                    break;

                case PipelineSection.Opportunities:
                    var opportunities = await _opportunitiesApiClient.GetAsync(
                        search,
                        NormalizeFilterValue(OpportunityStagePicker.SelectedItem),
                        CancellationToken.None);
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
        LeadStatusPicker.IsVisible = _selectedSection == PipelineSection.Leads;
        CustomerHealthPicker.IsVisible = _selectedSection == PipelineSection.Customers;
        OpportunityStagePicker.IsVisible = _selectedSection == PipelineSection.Opportunities;

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

    private async void OnSearchSubmitted(object? sender, EventArgs e) => await LoadAsync();

    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_filtersInitialized && string.IsNullOrWhiteSpace(e.NewTextValue) && !string.IsNullOrWhiteSpace(e.OldTextValue))
        {
            await LoadAsync();
        }
    }

    private async void OnLeadStatusChanged(object? sender, EventArgs e)
    {
        if (_filtersInitialized && _selectedSection == PipelineSection.Leads)
        {
            await LoadAsync();
        }
    }

    private async void OnCustomerHealthChanged(object? sender, EventArgs e)
    {
        if (_filtersInitialized && _selectedSection == PipelineSection.Customers)
        {
            await LoadAsync();
        }
    }

    private async void OnOpportunityStageChanged(object? sender, EventArgs e)
    {
        if (_filtersInitialized && _selectedSection == PipelineSection.Opportunities)
        {
            await LoadAsync();
        }
    }

    private async void OnCreateRecordClicked(object? sender, EventArgs e)
    {
        var action = await DisplayActionSheetAsync(
            "Novo cadastro",
            "Cancelar",
            null,
            "Lead",
            "Cliente",
            "Oportunidade");

        Page? page = action switch
        {
            "Lead" => _serviceProvider.GetRequiredService<CreateLeadPage>(),
            "Cliente" => _serviceProvider.GetRequiredService<CreateCustomerPage>(),
            "Oportunidade" => _serviceProvider.GetRequiredService<CreateOpportunityPage>(),
            _ => null
        };

        if (page is not null)
        {
            await Navigation.PushModalAsync(new NavigationPage(page));
        }
    }

    private void InitializeFilters()
    {
        LeadStatusPicker.ItemsSource = _leadStatusOptions.ToList();
        LeadStatusPicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        LeadStatusPicker.SelectedItem = _leadStatusOptions[0];

        CustomerHealthPicker.ItemsSource = _customerHealthOptions.ToList();
        CustomerHealthPicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        CustomerHealthPicker.SelectedItem = _customerHealthOptions[0];

        OpportunityStagePicker.ItemsSource = _opportunityStageOptions.ToList();
        OpportunityStagePicker.ItemDisplayBinding = new Binding(nameof(FormOption.Label));
        OpportunityStagePicker.SelectedItem = _opportunityStageOptions[0];

        _filtersInitialized = true;
    }

    private static string? NormalizeSearch(string? searchText) =>
        string.IsNullOrWhiteSpace(searchText) ? null : searchText.Trim();

    private static string? NormalizeFilterValue(object? selectedItem)
    {
        var value = (selectedItem as FormOption)?.Value;
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private enum PipelineSection
    {
        Leads,
        Customers,
        Opportunities
    }
}
