namespace UICrafter.Mobile.Platforms.Android;

using global::Android.App;
using global::Android.Content;
using Microsoft.Identity.Client;

[Activity(Exported = true)]
[IntentFilter([Intent.ActionView],
		Categories = [Intent.CategoryBrowsable, Intent.CategoryDefault],
		DataHost = "auth",
		DataScheme = "msalffacc574-3df3-471f-ae2b-71a6a42f1978")]
public class MsalActivity : BrowserTabActivity
{
}
