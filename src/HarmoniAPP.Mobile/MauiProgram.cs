using HarmoniAPP.Core.Services;
using HarmoniAPP.Mobile.Pages;
using HarmoniAPP.Mobile.Services;
using Microsoft.Extensions.Logging;

namespace HarmoniAPP.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var apiBaseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "https://10.0.2.2:7174/"
            : "https://localhost:7174/";

        builder.Services.AddSingleton(new ApiOptions
        {
            BaseUrl = apiBaseUrl
        });
        builder.Services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<ApiOptions>();
            return new HttpClient
            {
                BaseAddress = new Uri(options.BaseUrl),
                Timeout = TimeSpan.FromSeconds(20)
            };
        });
        builder.Services.AddSingleton<ITokenStore, SecureTokenStore>();
        builder.Services.AddSingleton<AuthApiClient>();
        builder.Services.AddSingleton<DashboardApiClient>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
