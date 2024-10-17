namespace UICrafter.Models;

using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Riok.Mapperly.Abstractions;
using UICrafter.Core.AppView;

public class AppViewEntity
{
	public int Id { get; set; }
	public required string UserId { get; set; }
	public required string Name { get; set; }
	public required byte[] Content { get; set; }
	public DateTime CreatedAtUTC { get; set; }
	public DateTime UpdatedAtUTC { get; set; }
}


[Mapper]
public partial class AppViewMapper
{
	// Map from AppViewEntity to gRPC AppView
	[MapProperty(nameof(AppViewEntity.CreatedAtUTC), nameof(AppView.CreatedAtUTC))]
	[MapProperty(nameof(AppViewEntity.UpdatedAtUTC), nameof(AppView.UpdatedAtUTC))]
	public partial AppView ToGrpcAppView(AppViewEntity entity);

	// Map from gRPC AppView to AppViewEntity
	[MapProperty(nameof(AppView.CreatedAtUTC), nameof(AppViewEntity.CreatedAtUTC))]
	[MapProperty(nameof(AppView.UpdatedAtUTC), nameof(AppViewEntity.UpdatedAtUTC))]
	public partial AppViewEntity ToAppViewEntity(AppView appView);

	// Custom mapping for converting DateTime to Timestamp
	private static Timestamp? MapCreatedAtUTC(DateTime createdAtUTC)
	{
		return Timestamp.FromDateTime(createdAtUTC.ToUniversalTime());
	}

	private static DateTime MapCreatedAtUTC(Timestamp createdAtUTC)
	{
		return createdAtUTC.ToDateTime();
	}

	// Custom mapping for converting DateTime to Timestamp for UpdatedAt
	private static Timestamp? MapUpdatedAtUTC(DateTime updatedAtURC)
	{
		return Timestamp.FromDateTime(updatedAtURC.ToUniversalTime());
	}

	private static DateTime MapUpdatedAtUTC(Timestamp updatedAtURC)
	{
		return updatedAtURC.ToDateTime();
	}

	// Custom mapping for converting byte[] to ByteString for AppViewEntity -> AppView
	private static ByteString MapContent(byte[] content)
	{
		return content != null ? ByteString.CopyFrom(content) : ByteString.Empty;
	}

	// Custom mapping for converting ByteString to byte[] for AppView -> AppViewEntity
	private static byte[] MapContent(ByteString content)
	{
		return content?.ToByteArray();
	}
}
