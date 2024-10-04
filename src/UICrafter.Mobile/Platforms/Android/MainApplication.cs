namespace UICrafter.Mobile.Platforms.Android;

using global::Android.App;
using global::Android.Runtime;

[Application]
public class MainApplication(nint handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
