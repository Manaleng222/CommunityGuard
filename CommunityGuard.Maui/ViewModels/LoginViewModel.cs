using CommunityGuard.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CommunityGuard.Maui.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    // ObservableProperty binds the text from the Entry controls in XAML to these variables.
    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    // Constructor receives the API service via Dependency Injection.
    public LoginViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    // [RelayCommand] generates a public ICommand property named LoginCommand.
    [RelayCommand]
    private async Task LoginAsync()
    {
        await ExecuteAsync(async () =>
        {
            // 1. Call the API.
            var result = await _apiService.LoginAsync(Email, Password);

            if (result != null)
            {
                // 2. Securely save the token. SecureStorage uses Windows Credentials Manager natively.
                await SecureStorage.Default.SetAsync("jwt_token", result.Token);
                await SecureStorage.Default.SetAsync("user_email", result.Email);
                await SecureStorage.Default.SetAsync("user_role", result.Role);

                // 3. Navigate to the Dashboard (we'll create this page next).
                // Shell navigation uses the route registered in AppShell.xaml.
                await Shell.Current.GoToAsync("//DashboardPage");
            }
        });
    }
}