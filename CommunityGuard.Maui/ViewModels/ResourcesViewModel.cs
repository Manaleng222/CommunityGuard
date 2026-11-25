using System.Collections.ObjectModel;
using CommunityGuard.Maui.Services;
using CommunityGuard.Shared.Dtos;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CommunityGuard.Maui.ViewModels;

public partial class ResourcesViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    public ObservableCollection<ResourceDto> Resources { get; } = new();
    public ObservableCollection<IncidentDto> OpenIncidents { get; } = new();

    [ObservableProperty]
    public partial ResourceDto? SelectedResource { get; set; }

    [ObservableProperty]
    public partial IncidentDto? SelectedIncident { get; set; }

    // Date and Time properties bound to UI Pickers
    [ObservableProperty] public partial DateTime SelectedDate { get; set; }
    [ObservableProperty] public partial TimeSpan StartTime { get; set; }
    [ObservableProperty] public partial TimeSpan EndTime { get; set; }

    public ResourcesViewModel(IApiService apiService)
    {
        _apiService = apiService;
        SelectedDate = DateTime.Today;
        StartTime = new TimeSpan(8, 0, 0); // Default 8 AM
        EndTime = new TimeSpan(12, 0, 0); // Default 12 PM
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            // 1. Load Resources
            var resResult = await _apiService.GetResourcesAsync();
            if (resResult != null)
            {
                Resources.Clear();
                foreach (var r in resResult) Resources.Add(r);
            }

            // 2. Load Incidents (UX: Only fetch OPEN incidents to dispatch to)
            var incResult = await _apiService.GetIncidentsAsync(1, 50); // Load top 50
            if (incResult != null)
            {
                OpenIncidents.Clear();
                // We filter out Closed incidents in the UI for better UX
                foreach (var i in incResult.Items.Where(x => x.Status != CommunityGuard.Domain.Enums.IncidentStatus.Closed))
                {
                    OpenIncidents.Add(i);
                }
            }
        });
    }

    [RelayCommand]
    private async Task AllocateAsync()
    {
        if (SelectedResource == null || SelectedIncident == null)
        {
            await Shell.Current.DisplayAlert("Hold on", "Please select both a resource and an incident.", "OK");
            return;
        }

        if (EndTime <= StartTime)
        {
            await Shell.Current.DisplayAlert("Invalid Time", "End time must be after start time.", "OK");
            return;
        }

        await ExecuteAsync(async () =>
        {
            // Combine Date and Time into UTC DateTime for the API
            var fromUtc = SelectedDate.Date.Add(StartTime).ToUniversalTime();
            var toUtc = SelectedDate.Date.Add(EndTime).ToUniversalTime();

            var request = new AllocateResourceRequest(SelectedIncident.Id, fromUtc, toUtc);

            await _apiService.AllocateResourceAsync(SelectedResource.Id, request);

            await Toast.Make("Resource successfully dispatched!", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();

            // Reset selections
            SelectedResource = null;
            SelectedIncident = null;
        });
    }
}
