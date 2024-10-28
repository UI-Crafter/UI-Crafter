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


	public async Task<IList<AppView>> GetAppViewsByUserIdAsync()
	{
		var response = await _appViewServiceClient.ListAppViewsAsync(new Empty());

		return response.AppViews;
	}

	public async Task<AppView> GetAppViewByIdAsync(long id) => await _appViewServiceClient.GetAppViewAsync(new GetAppViewRequest { Id = id });

	public async Task<AppView> CreateAppViewAsync(AppView view) => await _appViewServiceClient.CreateAppViewAsync(new CreateAppViewRequest { AppView = view });

	public async Task<AppView> UpdateAppViewAsync(AppView view) => await _appViewServiceClient.UpdateAppViewAsync(new UpdateAppViewRequest { AppView = view });

	public async Task DeleteAppViewByIdAsync(long id) => await _appViewServiceClient.DeleteAppViewAsync(new DeleteAppViewRequest { Id = id });
}
