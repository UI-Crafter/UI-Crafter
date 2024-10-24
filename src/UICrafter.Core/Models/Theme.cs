namespace UICrafter.Core.Models;

/// <summary>
/// Represents the theme settings for a user, including the preferred theme state.
/// </summary>
/// <remarks>
/// The <see cref="ThemeMode"/> can be set to always light, always dark, or follow the system/browser's automatic theme preferences.
/// </remarks>
public sealed class Theme
{
	/// <summary>
	/// Gets or sets the user's preferred theme state.
	/// The default value is <see cref="ThemeMode.Automatic"/>, which follows the system/browser's theme preference.
	/// </summary>
	public ThemeMode ThemeMode { get; set; } = ThemeMode.Automatic;
	/// <summary>
	/// The systems darkmode setting
	/// </summary>
	public bool SystemDarkMode { get; set; }
}



/// <summary>
/// Represents the available theme states for the application.
/// </summary>
/// <remarks>
/// The theme modes are treated as persistent states:
/// - <see cref="LightMode"/> is always light.
/// - <see cref="DarkMode"/> is always dark.
/// - <see cref="Automatic"/> follows the user's browser or system preferences.
/// </remarks>
public enum ThemeMode
{
	/// <summary>
	/// Represents the light mode state, where the theme is always light regardless of system or browser preferences.
	/// </summary>
	LightMode,

	/// <summary>
	/// Represents the dark mode state, where the theme is always dark regardless of system or browser preferences.
	/// </summary>
	DarkMode,

	/// <summary>
	/// Represents the automatic theme state, where the theme follows the browser's or system's dark mode or light mode preferences.
	/// </summary>
	Automatic
}
