// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UICrafter.Mobile.WinUI;

using Microsoft.UI.Xaml;
using UICrafter.Mobile.MSALClient;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		// configure redirect URI for your application
		PlatformConfig.Instance.RedirectUri = $"msal{PublicClientSingleton.Instance.MSALClientHelper.AzureAdConfig.ClientId}://auth";

		// Initialize MSAL
		PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppAsync().Wait();
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		base.OnLaunched(args);

		var app = UICrafter.Mobile.App.Current;
		PlatformConfig.Instance.ParentWindow = (app.Windows[0].Handler.PlatformView as MauiWinUIWindow)!.WindowHandle;
	}
}
