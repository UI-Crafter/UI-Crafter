namespace UICrafter.Mobile;

public partial class MainPage : ContentPage
{
	public MainPage() => InitializeComponent();

	protected override void OnHandlerChanged()
	{
		base.OnHandlerChanged();
#if ANDROID
		var blazorView = this.blazorWebView;
		var platformView = blazorView.Handler!.PlatformView as Android.Webkit.WebView;
		platformView!.OverScrollMode = Android.Views.OverScrollMode.Never;
#endif
	}
}
