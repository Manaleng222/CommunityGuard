using CommunityGuard.Maui.ViewModels;

namespace CommunityGuard.Maui.Views;

public partial class ResourcesPage : ContentPage
{
    public ResourcesPage(ResourcesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ResourcesViewModel vm)
        {
            vm.LoadDataCommand.Execute(null);
        }
    }
}