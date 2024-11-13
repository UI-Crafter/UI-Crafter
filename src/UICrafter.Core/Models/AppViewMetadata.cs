namespace UICrafter.Core.AppView;

using Google.Protobuf.WellKnownTypes;

public partial class AppViewMetadata
{
	// Expose CreatedAt as a DateTimeOffset property
	public DateTime CreatedAt
	{
		get => CreatedAtUTC?.ToDateTime() ?? DateTime.MinValue;
		set => CreatedAtUTC = value.ToTimestamp();
	}

	// Expose UpdatedAt as a DateTimeOffset property
	public DateTime UpdatedAt
	{
		get => UpdatedAtUTC?.ToDateTime() ?? DateTime.MinValue;
		set => UpdatedAtUTC = value.ToTimestamp();
	}
}
