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
        builder.Services.AddSingleton<AppNavigator>();
        builder.Services.AddSingleton<AuthApiClient>();
        builder.Services.AddSingleton<DashboardApiClient>();
        builder.Services.AddSingleton<LeadsApiClient>();
        builder.Services.AddSingleton<CustomersApiClient>();
        builder.Services.AddSingleton<OpportunitiesApiClient>();
        builder.Services.AddSingleton<TasksApiClient>();
        builder.Services.AddSingleton<InteractionsApiClient>();
        builder.Services.AddTransient<LaunchPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<PipelinePage>();
        builder.Services.AddTransient<TasksPage>();
        builder.Services.AddTransient<CreateTaskPage>();
        builder.Services.AddTransient<InteractionsPage>();
        builder.Services.AddTransient<CreateInteractionPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<AppTabPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
