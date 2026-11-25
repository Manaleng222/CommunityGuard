using CommunityGuard.Maui.ViewModels;

namespace CommunityGuard.Maui.Views;

public partial class DispatchPage : ContentPage
{
    public DispatchPage(DispatchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}