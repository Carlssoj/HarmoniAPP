using HarmoniAPP.Mobile.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace HarmoniAPP.Mobile;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        var culture = new CultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var startPage = _serviceProvider.GetRequiredService<LaunchPage>();
        return new Window(new NavigationPage(startPage));
    }
}
