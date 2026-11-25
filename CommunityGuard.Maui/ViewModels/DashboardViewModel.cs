using CommunityGuard.Maui.Services;
using CommunityGuard.Shared.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CommunityGuard.Maui.ViewModels;

// Receive the email passed during navigation
[QueryProperty(nameof(UserEmail), "Email")]
public partial class DashboardViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    public partial string? UserEmail { get; set; }

    [ObservableProperty]
    public partial DashboardStatsDto? Stats { get; set; }

    public DashboardViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadStatsAsync()
    {
        await ExecuteAsync(async () =>
        {
            Stats = await _apiService.GetDashboardStatsAsync();
        });
    }

    [RelayCommand]
    private async Task NavigateToIncidentsAsync()
    {
        await Shell.Current.GoToAsync("//IncidentsPage");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        SecureStorage.Default.RemoveAll();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}