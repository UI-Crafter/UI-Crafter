namespace UICrafter.Mobile.Platforms.Android;

using global::Android.App;
using global::Android.Content;
using Microsoft.Identity.Client;

[Activity(Exported = true)]
[IntentFilter([Intent.ActionView],
		Categories = [Intent.CategoryBrowsable, Intent.CategoryDefault],
		DataHost = "auth",
		DataScheme = "msal419ce4ca-106d-41bb-8567-0f75c75733dc")]
public class MsalActivity : BrowserTabActivity
{
}
