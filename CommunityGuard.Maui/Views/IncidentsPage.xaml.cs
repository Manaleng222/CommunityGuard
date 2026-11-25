using CommunityGuard.Maui.ViewModels;
using CommunityGuard.Shared.Dtos;

namespace CommunityGuard.Maui.Views;

public partial class IncidentsPage : ContentPage
{
    public IncidentsPage(IncidentsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is IncidentsViewModel vm && vm.Incidents.Count == 0)
        {
            vm.LoadIncidentsCommand.Execute(null);
        }
    }

    private void OnIncidentSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is IncidentDto selectedIncident)
        {
            if (BindingContext is IncidentsViewModel vm)
            {
                vm.LoadDetailsCommand.Execute(selectedIncident);
            }
        }
    }
}