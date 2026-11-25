using CommunityGuard.Maui.ViewModels;

namespace CommunityGuard.Maui.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        // Set the BindingContext to the injected ViewModel.
        BindingContext = viewModel;
    }
}