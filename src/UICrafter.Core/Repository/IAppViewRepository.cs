namespace UICrafter.Core.Repository;

using UICrafter.Core.AppView;

public interface IAppViewRepository
{
	Task<IList<AppViewMetadata>> GetAppViewMetadata();
	Task<AppView> GetAppViewById(long id);
	Task<AppView> CreateAppView(AppView view);
	Task<AppView> UpdateAppView(AppView view);
	Task DeleteAppViewById(long id);
}
