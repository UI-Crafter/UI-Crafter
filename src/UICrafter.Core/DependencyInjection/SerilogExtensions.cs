namespace UICrafter.Core.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

public static class SerilogExtensions
{
	public static ILoggingBuilder AddSeriloggerSetup(this ILoggingBuilder loggingBuilder, IConfiguration configuration)
	{
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.CreateLogger();

		loggingBuilder.ClearProviders();
		loggingBuilder.AddSerilog(Log.Logger, dispose: true);

		return loggingBuilder;
	}
}
