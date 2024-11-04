namespace UICrafter.Mobile.MSALClient;

using Microsoft.IdentityModel.Abstractions;

public class MyIdentityLogger : IIdentityLogger
{
	public EventLogLevel MinLogLevel { get; }

	public MyIdentityLogger()
	{
		//Retrieve the log level from an environment variable
		var msalEnvLogLevel = Environment.GetEnvironmentVariable("MSAL_LOG_LEVEL");

		if (Enum.TryParse(msalEnvLogLevel, out EventLogLevel msalLogLevel))
		{
			MinLogLevel = msalLogLevel;
		}
		else
		{
			//Recommended default log level
			MinLogLevel = EventLogLevel.Informational;
		}
	}

	public bool IsEnabled(EventLogLevel eventLogLevel)
	{
		return eventLogLevel <= MinLogLevel;
	}

	public void Log(LogEntry entry)
	{
		GetLogLevel(entry).Invoke(entry.Message);
	}

	private static Action<string> GetLogLevel(LogEntry entry) => entry.EventLogLevel switch
	{
		EventLogLevel.Critical => Serilog.Log.Fatal,
		EventLogLevel.Error => Serilog.Log.Error,
		EventLogLevel.Warning => Serilog.Log.Warning,
		EventLogLevel.Informational => Serilog.Log.Information,
		EventLogLevel.Verbose => Serilog.Log.Verbose,
		EventLogLevel.LogAlways => Serilog.Log.Information,
		_ => Serilog.Log.Information
	};
}
