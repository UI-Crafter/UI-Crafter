namespace UICrafter.Mobile.Platforms.iOS;

using Foundation;
using UICrafter.Mobile.MSALClient;
using UIKit;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
	{
		// configure platform specific params
		PlatformConfig.Instance.RedirectUri = $"msal{PublicClientSingleton.Instance.MSALClientHelper.AzureAdConfig.ClientId}://auth";

		// Initialize MSAL and platformConfig is set
		PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppAsync().Wait();

		return base.FinishedLaunching(application, launchOptions);
	}
}
