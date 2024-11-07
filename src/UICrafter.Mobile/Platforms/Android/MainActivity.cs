namespace UICrafter.Mobile.Platforms.Android;

using global::Android.App;
using global::Android.Content;
using global::Android.Content.PM;
using global::Android.OS;
using Microsoft.Identity.Client;
using UICrafter.Mobile.MSALClient;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		// configure platform specific params
		PlatformConfig.Instance.RedirectUri = $"msal{PublicClientSingleton.Instance.MSALClientHelper.AzureAdConfig.ClientId}://auth";
		PlatformConfig.Instance.ParentWindow = this;

		// Initialize MSAL and platformConfig is set
		PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppAsync().Wait();
	}

	protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
	{
		base.OnActivityResult(requestCode, resultCode, data);
		AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
	}
}
