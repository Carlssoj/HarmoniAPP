using Microsoft.Extensions.DependencyInjection;
using HarmoniAPP.Mobile.Pages;

namespace HarmoniAPP.Mobile;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var startPage = _serviceProvider.GetRequiredService<LoginPage>();
        return new Window(new NavigationPage(startPage));
    }
}
