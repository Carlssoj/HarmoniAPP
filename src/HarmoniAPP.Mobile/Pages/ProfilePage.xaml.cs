using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Services;

namespace HarmoniAPP.Mobile.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly AuthApiClient _authApiClient;
    private readonly AppNavigator _appNavigator;
    private readonly ApiOptions _apiOptions;

    public ProfilePage(AuthApiClient authApiClient, AppNavigator appNavigator, ApiOptions apiOptions)
    {
        _authApiClient = authApiClient;
        _appNavigator = appNavigator;
        _apiOptions = apiOptions;
        InitializeComponent();

        AppVersionLabel.Text = $"Versão {AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";
        ApiBaseUrlLabel.Text = _apiOptions.BaseUrl;
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
            StatusLabel.IsVisible = false;

            var user = await _authApiClient.GetMeAsync();
            if (user is null)
            {
                await _appNavigator.ShowLoginAsync();
                return;
            }

            DisplayNameLabel.Text = user.DisplayName;
            AccessLabel.Text = $"{user.Profile} · {user.Team}";
            VisibilityLabel.Text = user.HasCrossTeamVisibility
                ? "Acesso multi-equipes"
                : "Acesso ao próprio time";
        }
        catch (Exception exception)
        {
            StatusLabel.Text = MobileErrorFormatter.Format(exception);
            StatusLabel.IsVisible = true;
        }
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        await _authApiClient.LogoutAsync();
        await _appNavigator.ShowLoginAsync();
    }
}
