namespace CommunityGuard.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register the detail page route
        //Routing.RegisterRoute(nameof(Views.IncidentDetailPage), typeof(Views.IncidentDetailPage));

        // Listen for navigation events to update UI visibility
        Navigating += OnShellNavigating;
    }

    private async void OnShellNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        // If they are going to the login page, do nothing
        if (e.Target.Location.OriginalString.Contains("LoginPage")) return;

        // Fetch the role securely
        var role = await SecureStorage.Default.GetAsync("user_role");

        // UX RULE: Only "Controllers" can create incidents or dispatch resources.
        bool isController = role == "Controller";

        DispatchMenuItem.IsVisible = isController;
        ResourcesMenuItem.IsVisible = isController;
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        SecureStorage.Default.RemoveAll();
        await GoToAsync("//LoginPage");
        FlyoutIsPresented = false;
    }
}