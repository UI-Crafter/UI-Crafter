namespace UICrafter.Mobile;

using UICrafter.Mobile.Utility;

public partial class App : Application
{
	public App(RefreshViewState refreshViewState)
	{
		InitializeComponent();

		MainPage = new MainPage(refreshViewState);
	}
}
