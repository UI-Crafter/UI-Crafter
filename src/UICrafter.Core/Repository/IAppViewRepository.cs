namespace UICrafter.Core.Repository;

using UICrafter.Core.AppView;

public interface IAppViewRepository
{
	Task<IList<AppView>> GetAppViewsByUserIdAsync(Guid userId);
	Task<AppView> GetAppViewByIdAsync(long id);
	Task<AppView> CreateAppViewAsync(AppView view);
	Task<AppView> UpdateAppViewAsync(AppView view);
	Task DeleteAppViewByIdAsync(long id);
}
