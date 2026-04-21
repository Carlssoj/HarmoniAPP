using HarmoniAPP.Core.Models.Auth;
using HarmoniAPP.Core.Models.Dashboard;
using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Services;

namespace HarmoniAPP.Mobile.Pages;

public partial class HomePage : ContentPage
{
    private readonly AuthApiClient _authApiClient;
    private readonly DashboardApiClient _dashboardApiClient;
    private readonly AppNavigator _appNavigator;
    private bool _isLoading;

    public HomePage(
        AuthApiClient authApiClient,
        DashboardApiClient dashboardApiClient,
        AppNavigator appNavigator)
    {
        _authApiClient = authApiClient;
        _dashboardApiClient = dashboardApiClient;
        _appNavigator = appNavigator;
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

            var user = await _authApiClient.GetMeAsync();
            if (user is null)
            {
                await _appNavigator.ShowLoginAsync();
                return;
            }

            var summary = await _dashboardApiClient.GetResumoAsync();
            ApplyUser(user);
            ApplySummary(summary);
        }
        catch (Exception exception)
        {
            StatusLabel.Text = MobileErrorFormatter.Format(exception);
            StatusLabel.IsVisible = true;
        }
        finally
        {
            DashboardRefreshView.IsRefreshing = false;
            _isLoading = false;
        }
    }

    private void ApplyUser(AuthenticatedUserResponse user)
    {
        GreetingLabel.Text = $"Olá, {user.DisplayName}";
        ProfileLabel.Text = $"{user.Profile} · {user.Team}";
    }

    private void ApplySummary(DashboardSummaryResponse? summary)
    {
        if (summary is null)
        {
            SummaryLabel.Text = "Ainda não foi possível carregar os indicadores principais.";
            return;
        }

        PipelineValueLabel.Text = summary.PipelineAberto.ToString("C0");
        MonthlyRevenueValueLabel.Text = summary.ReceitaMensal.ToString("C0");
        SummaryLabel.Text = $"{summary.TarefasPendentes} tarefa(s) pedem atenção e {summary.InteracoesUltimos7Dias} interação(ões) aconteceram na última semana.";

        LeadsValueLabel.Text = summary.Leads.ToString();
        CustomersValueLabel.Text = summary.Clientes.ToString();
        OpportunitiesValueLabel.Text = summary.OportunidadesAbertas.ToString();
        TasksValueLabel.Text = summary.TarefasPendentes.ToString();
        InteractionsValueLabel.Text = summary.InteracoesUltimos7Dias.ToString();
    }

    private async void OnRefreshRequested(object? sender, EventArgs e) => await LoadAsync();

    private void SelectTab(int index)
    {
        if (Parent is NavigationPage navigationPage && navigationPage.Parent is AppTabPage tabPage)
        {
            tabPage.SelectTab(index);
        }
    }

    private void OnOpenPipelineClicked(object? sender, EventArgs e) => SelectTab(1);

    private void OnOpenTasksClicked(object? sender, EventArgs e) => SelectTab(2);

    private void OnOpenInteractionsClicked(object? sender, EventArgs e) => SelectTab(3);
}
