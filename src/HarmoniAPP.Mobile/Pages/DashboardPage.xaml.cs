using HarmoniAPP.Core.Models.Auth;
using HarmoniAPP.Core.Models.Dashboard;
using HarmoniAPP.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HarmoniAPP.Mobile.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardApiClient _dashboardApiClient;
    private readonly AuthApiClient _authApiClient;
    private readonly IServiceProvider _serviceProvider;

    public DashboardPage(
        DashboardApiClient dashboardApiClient,
        AuthApiClient authApiClient,
        IServiceProvider serviceProvider)
    {
        _dashboardApiClient = dashboardApiClient;
        _authApiClient = authApiClient;
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
        try
        {
            var me = await _authApiClient.GetMeAsync();
            var summary = await _dashboardApiClient.GetResumoAsync();

            ApplyUser(me);
            ApplySummary(summary);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Falha", $"Não foi possível carregar o dashboard: {ex.Message}", "OK");
        }
    }

    private void ApplyUser(AuthenticatedUserResponse? user)
    {
        WelcomeLabel.Text = user is null ? "Dashboard Harmoni" : $"Olá, {user.DisplayName}";
        ProfileLabel.Text = user is null
            ? "Sessão mobile"
            : $"{user.Profile} · {user.Team}";
    }

    private void ApplySummary(DashboardSummaryResponse? summary)
    {
        LeadsLabel.Text = summary?.Leads.ToString() ?? "-";
        CustomersLabel.Text = summary?.Clientes.ToString() ?? "-";
        OpportunitiesLabel.Text = summary?.OportunidadesAbertas.ToString() ?? "-";
        TasksLabel.Text = summary?.TarefasPendentes.ToString() ?? "-";
        PipelineLabel.Text = summary is null ? "-" : summary.PipelineAberto.ToString("C");
    }

    private Task OpenModuleAsync(string title, string description) =>
        Navigation.PushAsync(new ModulePlaceholderPage(title, description));

    private async void OnOpenLeadsClicked(object? sender, EventArgs e) =>
        await OpenModuleAsync("Leads", "Consulta de leads do time via /api/v1/leads.");

    private async void OnOpenCustomersClicked(object? sender, EventArgs e) =>
        await OpenModuleAsync("Clientes", "Consulta de clientes via /api/v1/clientes.");

    private async void OnOpenOpportunitiesClicked(object? sender, EventArgs e) =>
        await OpenModuleAsync("Oportunidades", "Consulta de oportunidades via /api/v1/oportunidades.");

    private async void OnOpenTasksClicked(object? sender, EventArgs e) =>
        await OpenModuleAsync("Tarefas", "Agenda e criação de tarefas via /api/v1/tarefas.");

    private async void OnOpenInteractionsClicked(object? sender, EventArgs e) =>
        await OpenModuleAsync("Interações", "Histórico e cadastro via /api/v1/interacoes.");

    private async void OnOpenProfileClicked(object? sender, EventArgs e) =>
        await OpenModuleAsync("Perfil", "Leitura do usuário autenticado via /api/v1/auth/me.");

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        await _authApiClient.LogoutAsync();
        var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
        await Navigation.PushAsync(loginPage);
        Navigation.RemovePage(this);
    }
}
