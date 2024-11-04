namespace UICrafter.Mobile.MSALClient;

public class DownstreamApiHelper
{
	private string[] DownstreamApiScopes;
	public DownStreamApiConfig DownstreamApiConfig;
	private MSALClientHelper MSALClient;

	public DownstreamApiHelper(DownStreamApiConfig downstreamApiConfig, MSALClientHelper msalClientHelper)
	{
		ArgumentNullException.ThrowIfNull(msalClientHelper);

		this.DownstreamApiConfig = downstreamApiConfig;
		this.MSALClient = msalClientHelper;
		this.DownstreamApiScopes = this.DownstreamApiConfig.ScopesArray;
	}
}
