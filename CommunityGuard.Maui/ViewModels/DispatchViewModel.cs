using CommunityGuard.Domain.Enums;
using CommunityGuard.Maui.Services;
using CommunityGuard.Shared.Dtos;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;



namespace CommunityGuard.Maui.ViewModels;

public partial class DispatchViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial string Description { get; set; }

    [ObservableProperty]
    public partial string Sector { get; set; }

    [ObservableProperty]
    public partial IncidentPriority SelectedPriority { get; set; }

    // Populate the Picker with Enum values
    public Array Priorities => Enum.GetValues(typeof(IncidentPriority));

    public DispatchViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = string.Empty;
        Description = string.Empty;
        Sector = string.Empty;
        SelectedPriority = IncidentPriority.Medium;
    }

    [RelayCommand]
    private async Task SubmitReportAsync()
    {
        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Sector))
        {
            await Shell.Current.DisplayAlert("Validation", "Title and Sector are required.", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            var request = new CreateIncidentRequest(Title, Description, Sector, SelectedPriority);
            await _apiService.CreateIncidentAsync(request);

            await Shell.Current.DisplayAlert("Success", "Incident reported successfully!", "OK");
            await Shell.Current.GoToAsync(".."); // Go back to the previous page
        });

        await ExecuteAsync(async () =>
        {
            var request = new CreateIncidentRequest(Title, Description, Sector, SelectedPriority);
            await _apiService.CreateIncidentAsync(request);

            // Show a professional Toast instead of an Alert
            var toast = Toast.Make("Incident reported successfully!", ToastDuration.Short, 14);
            await toast.Show();

            await Shell.Current.GoToAsync(".."); // Go back
        });
    }
}
