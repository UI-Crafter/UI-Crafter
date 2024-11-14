namespace UICrafter.Repository;

using Microsoft.EntityFrameworkCore;
using UICrafter.Core.AppView;
using UICrafter.Core.Extensions;
using UICrafter.Core.Repository;
using UICrafter.EntityConfigurations;
using UICrafter.Models;

public class AppViewRepository : IAppViewRepository
{
	private readonly ApplicationDbContext _dbContext;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly AppViewMapper _appViewMapper = new();

	public AppViewRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
	{
		_dbContext = dbContext;
		_httpContextAccessor = httpContextAccessor;
	}

	public async Task<IList<AppViewMetadata>> GetAppViewMetadata()
	{
		var userId = _httpContextAccessor.HttpContext!.User.GetAzureId();

		return await _dbContext
			.AppViews
			.AsNoTracking()
			.Where(x => x.UserId == userId || x.IsPublic)
			.Join(_dbContext.Users,
				appView => appView.UserId,
				user => user.Id,
				(appView, user) => new AppViewMetadata
				{
					Id = appView.Id,
					UserId = appView.UserId.ToString(),
					Name = appView.Name,
					CreatedAtUTC = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(appView.CreatedAtUTC),
					UpdatedAtUTC = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(appView.UpdatedAtUTC),
					UserName = user.Name,
					IsPublic = appView.IsPublic,
				}
			)
			.ToListAsync();
	}

	public async Task<AppView> GetAppViewById(long id)
	{
		var user = _httpContextAccessor.HttpContext!.User;
		var entity = await _dbContext.AppViews
			.AsNoTracking()
			.Where(x => x.Id == id && (x.IsPublic || x.UserId == user.GetAzureId()))
			.FirstAsync();

		return _appViewMapper.ToGrpcAppView(entity);
	}

	public async Task<AppView> CreateAppView(AppView view)
	{
		if (string.IsNullOrWhiteSpace(view.Name))
		{
			throw new ArgumentException("App-view missing name");
		}

		if (view.Content.Length == 0)
		{
			throw new ArgumentException("App-view contains no components");
		}

		var entity = _appViewMapper.ToAppViewEntity(view);

		var userId = _httpContextAccessor.HttpContext?.User.GetAzureId();
		if (!userId.HasValue)
		{
			throw new UnauthorizedAccessException("User is not authenticated.");
		}

		entity.Id = 0;
		entity.UserId = userId.Value;
		entity.CreatedAtUTC = DateTime.UtcNow;
		entity.UpdatedAtUTC = DateTime.UtcNow;

		_dbContext.AppViews.Add(entity);
		await _dbContext.SaveChangesAsync();

		return _appViewMapper.ToGrpcAppView(entity);
	}

	public async Task<AppView> UpdateAppView(AppView view)
	{
		if (string.IsNullOrWhiteSpace(view.Name))
		{
			throw new ArgumentException("App-view missing name");
		}

		if (view.Content.Length == 0)
		{
			throw new ArgumentException("App-view contains no components");
		}

		var userId = _httpContextAccessor.HttpContext!.User.GetAzureId();
		var entity = await _dbContext.AppViews
			.Where(x => x.Id == view.Id && x.UserId == userId) // Are you the owner check
			.FirstAsync();

		entity.Name = view.Name;
		entity.Content = view.ContentByteArray;
		entity.UpdatedAtUTC = DateTime.UtcNow;
		entity.IsPublic = view.IsPublic;

		await _dbContext.SaveChangesAsync();

		return _appViewMapper.ToGrpcAppView(entity);
	}

	public async Task DeleteAppViewById(long id)
	{
		var userId = _httpContextAccessor.HttpContext!.User.GetAzureId();
		await _dbContext.AppViews
			.Where(x => x.Id == id && x.UserId == userId)
			.ExecuteDeleteAsync();
	}
}
