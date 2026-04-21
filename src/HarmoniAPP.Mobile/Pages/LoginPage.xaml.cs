using HarmoniAPP.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HarmoniAPP.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthApiClient _authApiClient;
    private readonly IServiceProvider _serviceProvider;

    public LoginPage(AuthApiClient authApiClient, IServiceProvider serviceProvider)
    {
        _authApiClient = authApiClient;
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        try
        {
            StatusLabel.Text = "Autenticando...";
            var login = LoginEntry.Text?.Trim() ?? string.Empty;
            var password = PasswordEntry.Text ?? string.Empty;

            await _authApiClient.LoginAsync(login, password);
            var dashboardPage = _serviceProvider.GetRequiredService<DashboardPage>();
            await Navigation.PushAsync(dashboardPage);
            Navigation.RemovePage(this);
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Falha no login: {ex.Message}";
        }
    }
}
