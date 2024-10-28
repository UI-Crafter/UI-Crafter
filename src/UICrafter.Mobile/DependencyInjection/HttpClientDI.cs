namespace UICrafter.Mobile.DependencyInjection;

using Microsoft.Extensions.Options;
using UICrafter.Mobile.Options;

public static class HttpClientDI
{
	/// <summary>
	/// Register HttpClient using ApiSettings configuration
	/// </summary>
	/// <param name="services"></param>
	/// <param name="config"></param>
	/// <returns></returns>
	public static IServiceCollection AddConfiguredHttpClient(this IServiceCollection services)
	{
		// Register HttpClient using the configured BaseUrl from ApiSettings
		services.AddSingleton(sp =>
		{
			var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
			return new HttpClient { BaseAddress = new Uri(apiSettings.BaseUrl) };
		});

		return services;
	}
}
