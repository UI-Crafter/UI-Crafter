namespace UICrafter.Client.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using UICrafter.Core.AppView;
using UICrafter.Core.Repository;
using static UICrafter.Core.AppView.AppViewService;

public class AppViewRepository : IAppViewRepository
{
	private readonly AppViewServiceClient _appViewServiceClient;

	public AppViewRepository(AppViewServiceClient appViewServiceClient) => _appViewServiceClient = appViewServiceClient;

	public async Task<IList<AppViewMetadata>> GetAppViewMetadata()
	{
		var response = await _appViewServiceClient.ListAppViewMetadataAsync(new Empty());

		return response.AppViews;
	}

	public async Task<AppView> GetAppViewById(long id) => await _appViewServiceClient.GetAppViewAsync(new GetAppViewRequest { Id = id });

	public async Task<AppView> CreateAppView(AppView view) => await _appViewServiceClient.CreateAppViewAsync(new CreateAppViewRequest { AppView = view });

	public async Task<AppView> UpdateAppView(AppView view) => await _appViewServiceClient.UpdateAppViewAsync(new UpdateAppViewRequest { AppView = view });

	public async Task DeleteAppViewById(long id) => await _appViewServiceClient.DeleteAppViewAsync(new DeleteAppViewRequest { Id = id });

	
}
