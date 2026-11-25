using CommunityGuard.Maui.Services;
using CommunityGuard.Maui.ViewModels;
using CommunityGuard.Maui.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace CommunityGuard.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() // Initialize CommunityToolkit
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });



        // 1. REGISTER HTTP CLIENT
        // We register HttpClient with a BaseAddress. The API runs on localhost:7xxx.
        // IMPORTANT: Ensure this port matches your CommunityGuard.Api launchSettings.json!
        // 1. REGISTER HTTP CLIENT
        builder.Services.AddHttpClient<IApiService, ApiService>(client =>
        {
            // Use 127.0.0.1 instead of localhost to avoid IPv6 DNS issues
            client.BaseAddress = new Uri("https://localhost:7209/");
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();

            // BYPASS SSL ERRORS FOR LOCAL DEVELOPMENT ONLY
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true; // Trust local development certs
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
        });
        /// 2. REGISTER VIEWMODELS (Add ALL of them here)
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<IncidentsViewModel>();
        builder.Services.AddTransient<DispatchViewModel>();
        builder.Services.AddTransient<ResourcesViewModel>();

        // 3. REGISTER VIEWS (Add ALL pages here)
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<IncidentsPage>();
        builder.Services.AddTransient<DispatchPage>();
        builder.Services.AddTransient<ResourcesPage>(); ;
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }


}
