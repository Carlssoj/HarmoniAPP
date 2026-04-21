using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Services;

namespace HarmoniAPP.Mobile.Pages;

public partial class LaunchPage : ContentPage
{
    private readonly AuthApiClient _authApiClient;
    private readonly AppNavigator _appNavigator;
    private bool _initialized;

    public LaunchPage(AuthApiClient authApiClient, AppNavigator appNavigator)
    {
        _authApiClient = authApiClient;
        _appNavigator = appNavigator;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_initialized)
        {
            return;
        }

        _initialized = true;

        try
        {
            var user = await _authApiClient.GetMeAsync();
            if (user is null)
            {
                await _appNavigator.ShowLoginAsync();
                return;
            }

            await _appNavigator.ShowAuthenticatedAsync();
        }
        catch (Exception)
        {
            StatusLabel.Text = "Sessão não encontrada. Abrindo login...";
            await _authApiClient.LogoutAsync();
            await _appNavigator.ShowLoginAsync();
        }
    }
}
