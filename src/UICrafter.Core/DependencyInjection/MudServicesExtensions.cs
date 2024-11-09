namespace UICrafter.Core.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

public static class MudServicesExtensions
{
	public static IServiceCollection AddUICrafterMudServices(this IServiceCollection services)
	{
		return services.AddMudServices(config =>
		{
			config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

			config.SnackbarConfiguration.PreventDuplicates = true;
			config.SnackbarConfiguration.NewestOnTop = false;
			config.SnackbarConfiguration.ShowCloseIcon = true;
			config.SnackbarConfiguration.VisibleStateDuration = 1500;
			config.SnackbarConfiguration.HideTransitionDuration = 200;
			config.SnackbarConfiguration.ShowTransitionDuration = 200;
			config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
		});
	}
}
