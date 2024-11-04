namespace UICrafter.Mobile.Platforms.Android;

using global::Android.App;
using global::Android.Content;
using Microsoft.Identity.Client;

[Activity(Exported = true)]
[IntentFilter([Intent.ActionView],
		Categories = [Intent.CategoryBrowsable, Intent.CategoryDefault],
		DataHost = "auth",
		DataScheme = "msal3469f319-54f9-42d5-b2af-4d24c06994dc")]
public class MsalActivity : BrowserTabActivity
{
}
