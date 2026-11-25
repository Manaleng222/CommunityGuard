using System.Collections.ObjectModel;
using CommunityGuard.Maui.Services;
using CommunityGuard.Shared.Dtos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CommunityGuard.Maui.ViewModels;

public partial class IncidentsViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private int _currentPage = 1;
    private const int PageSize = 10;

    // The list for the left pane
    public ObservableCollection<IncidentDto> Incidents { get; } = new();

    // The selected item for the right pane
    [ObservableProperty]
    private IncidentDetailDto? _selectedIncidentDetail;

    [ObservableProperty]
    private bool _isRefreshing;

    public IncidentsViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadIncidentsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        IsRefreshing = true;

        try
        {
            var result = await _apiService.GetIncidentsAsync(_currentPage, PageSize);

            if (result != null)
            {
                Incidents.Clear();
                foreach (var incident in result.Items)
                {
                    Incidents.Add(incident);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task LoadDetailsAsync(IncidentDto selectedIncident)
    {
        if (selectedIncident == null) return;

        await ExecuteAsync(async () =>
        {
            // Fetch the full details (including history) for the right pane
            SelectedIncidentDetail = await _apiService.GetIncidentByIdAsync(selectedIncident.Id);
        });
    }
}