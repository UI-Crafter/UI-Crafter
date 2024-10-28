namespace UICrafter.Core.DependencyInjection;

using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.Extensions.DependencyInjection;

public static class GrpcClientDI
{
	/// <summary>
	/// Register gRPC Client using httpClient BaseAddress
	/// </summary>
	/// <typeparam name="TGrpcClient"></typeparam>
	/// <param name="services"></param>
	/// <returns></returns>
	public static IServiceCollection AddGrpcClient<TGrpcClient>(this IServiceCollection services) where TGrpcClient : ClientBase
	{
		// Register gRPC client using the BaseUrl from ApiSettings
		services.AddScoped(sp =>
		{
			var httpClient = sp.GetRequiredService<HttpClient>();
			var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions
			{
				HttpHandler = new GrpcWebHandler(new HttpClientHandler())
			});
			return (TGrpcClient)Activator.CreateInstance(typeof(TGrpcClient), channel)!;
		});

		return services;
	}
}
