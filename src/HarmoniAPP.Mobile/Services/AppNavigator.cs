using HarmoniAPP.Mobile.Pages;

namespace HarmoniAPP.Mobile.Services;

public sealed class AppNavigator
{
    private readonly IServiceProvider _serviceProvider;

    public AppNavigator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task ShowLoginAsync()
    {
        var page = _serviceProvider.GetRequiredService<LoginPage>();
        var navigationPage = new NavigationPage(page);
        NavigationPage.SetHasNavigationBar(page, false);
        GetWindow().Page = navigationPage;
        return Task.CompletedTask;
    }

    public Task ShowAuthenticatedAsync()
    {
        var page = _serviceProvider.GetRequiredService<AppTabPage>();
        GetWindow().Page = page;
        return Task.CompletedTask;
    }

    private static Window GetWindow() =>
        Application.Current?.Windows.FirstOrDefault()
        ?? throw new InvalidOperationException("Nenhuma janela ativa foi encontrada.");
}
