namespace UICrafter.Core.AppView;

using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

public partial class AppView
{
	// Expose CreatedAt as a DateTimeOffset property
	public DateTime CreatedAtOffset
	{
		get => CreatedAtUTC?.ToDateTime() ?? DateTime.MinValue;
		set => CreatedAtUTC = value.ToTimestamp();
	}

	// Expose UpdatedAt as a DateTimeOffset property
	public DateTime UpdatedAtOffset
	{
		get => UpdatedAtUTC?.ToDateTime() ?? DateTime.MinValue;
		set => UpdatedAtUTC = value.ToTimestamp();
	}

	public byte[] ContentByteArray
	{
		get => Content.ToByteArray();
		set => Content = ByteString.CopyFrom(value);
	}
}
