namespace UICrafter.Services;

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using UICrafter.Core.AppView;
using UICrafter.Core.Repository;

public class AppViewServicegRPC : AppViewService.AppViewServiceBase
{
	private readonly IAppViewRepository _appViewRepository;

	public AppViewServicegRPC(IAppViewRepository appViewRepository) => _appViewRepository = appViewRepository;

	public override async Task<ListAppViewsResponse> ListAppViews(Empty request, ServerCallContext context)
	{
		return new ListAppViewsResponse
		{
			AppViews = { await _appViewRepository.GetAppViews() }
		};
	}

	public override Task<AppView> GetAppView(GetAppViewRequest request, ServerCallContext context) => _appViewRepository.GetAppViewById(request.Id);
	public override Task<AppView> CreateAppView(CreateAppViewRequest request, ServerCallContext context) => _appViewRepository.CreateAppView(request.AppView);
	public override Task<AppView> UpdateAppView(UpdateAppViewRequest request, ServerCallContext context) => _appViewRepository.UpdateAppView(request.AppView);
	public override async Task<Empty> DeleteAppView(DeleteAppViewRequest request, ServerCallContext context)
	{
		await _appViewRepository.DeleteAppViewById(request.Id);
		return new Empty();
	}
}
