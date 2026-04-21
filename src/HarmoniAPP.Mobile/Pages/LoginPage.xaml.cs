using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Services;

namespace HarmoniAPP.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthApiClient _authApiClient;
    private readonly AppNavigator _appNavigator;

    public LoginPage(AuthApiClient authApiClient, AppNavigator appNavigator)
    {
        _authApiClient = authApiClient;
        _appNavigator = appNavigator;
        InitializeComponent();
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        try
        {
            var login = LoginEntry.Text?.Trim() ?? string.Empty;
            var password = PasswordEntry.Text ?? string.Empty;

            LoginButton.IsEnabled = false;
            StatusLabel.IsVisible = false;

            await _authApiClient.LoginAsync(login, password);
            await _appNavigator.ShowAuthenticatedAsync();
        }
        catch (Exception ex)
        {
            StatusLabel.Text = MobileErrorFormatter.Format(ex);
            StatusLabel.IsVisible = true;
        }
        finally
        {
            LoginButton.IsEnabled = true;
        }
    }
}
