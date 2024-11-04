// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace UICrafter.Mobile.MSALClient;

using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;

/// <summary>
/// This is a singleton implementation to wrap the MSALClient and associated classes to support static initialization model for platforms that need 
/// </summary>
public class PublicClientSingleton
{
	/// <summary>
	/// This is the singleton used by Ux. Since PublicClientWrapper constructor does not have perf or memory issue, it is instantiated directly.
	/// </summary>
	public static PublicClientSingleton Instance { get; private set; } = new PublicClientSingleton();

	/// <summary>
	/// This is the configuration for the application found within the 'appsettings.json' file.
	/// </summary>
	private static IConfiguration _appConfiguration = default!;

	/// <summary>
	/// Gets the instance of MSALClientHelper.
	/// </summary>
	public DownStreamApiConfig DownStreamApiConfig { get; }

	/// <summary>
	/// Gets the instance of MSALClientHelper.
	/// </summary>
	public MSALClientHelper MSALClientHelper { get; }


	//// Custom logger for sample
	//private readonly IdentityLogger _logger = new IdentityLogger();

	/// <summary>
	/// Prevents a default instance of the <see cref="PublicClientSingleton"/> class from being created. or a private constructor for singleton
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private PublicClientSingleton()
	{
		// Load config
		var assembly = Assembly.GetExecutingAssembly();
		using var stream = assembly.GetManifestResourceStream("UICrafter.Mobile.appsettings.json");
		_appConfiguration = new ConfigurationBuilder()
			.AddJsonStream(stream!)
			.Build();

		var azureADConfig = _appConfiguration.GetSection("AzureAd").Get<AzureAdConfig>()!;
		MSALClientHelper = new MSALClientHelper(azureADConfig);

		DownStreamApiConfig = _appConfiguration.GetSection("DownstreamApi").Get<DownStreamApiConfig>()!;
	}

	/// <summary>
	/// Gets scopes for the application
	/// </summary>
	/// <returns>An array of all scopes</returns>
	internal string[]? GetScopes()
	{
		return DownStreamApiConfig.ScopesArray;
	}
}
