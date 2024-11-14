namespace UICrafter.Mobile.Utility;

using System.ComponentModel;
using Microsoft.Maui.Storage;
using UICrafter.Core.Models;

public partial class ThemeService : INotifyPropertyChanged
{
	private const string _themePreferenceKey = "ThemeMode";
	private ThemeMode _themeMode;

	public event PropertyChangedEventHandler? PropertyChanged;

	public ThemeService() => _themeMode = (ThemeMode)Preferences.Get(_themePreferenceKey, (int)ThemeMode.Automatic);

	public ThemeMode ThemeMode
	{
		get => _themeMode;
		private set
		{
			if (_themeMode != value)
			{
				_themeMode = value;
				Preferences.Set(_themePreferenceKey, (int)_themeMode);
				OnPropertyChanged(nameof(ThemeMode));
			}
		}
	}

	public void SetThemeMode(ThemeMode mode)
	{
		ThemeMode = mode;
	}

	public ThemeMode GetThemeMode()
	{
		return ThemeMode;
	}

	protected virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
