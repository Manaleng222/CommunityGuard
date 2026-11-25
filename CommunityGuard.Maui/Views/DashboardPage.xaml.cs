using CommunityGuard.Maui.ViewModels;

namespace CommunityGuard.Maui.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Trigger the load when the page appears
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DashboardViewModel vm && vm.Stats == null)
        {
            vm.LoadStatsCommand.Execute(null);
        }
    }
}