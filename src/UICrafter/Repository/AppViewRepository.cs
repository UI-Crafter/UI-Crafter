namespace UICrafter.Repository;

using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using UICrafter.Core.AppView;
using UICrafter.Core.Repository;
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

	private static readonly Func<ApplicationDbContext, string, Task<List<AppView>>> _listAppViewsQuery =
	EF.CompileAsyncQuery((ApplicationDbContext dbContext, string userId) =>
		dbContext.AppViews
			.Where(x => x.UserId == userId)
			.AsNoTracking()
			.Select(view => new AppView
			{
				Id = view.Id,
				UserId = view.UserId,
				Name = view.Name,
				CreatedAtUTC = Timestamp.FromDateTime(view.CreatedAtUTC),
				UpdatedAtUTC = Timestamp.FromDateTime(view.UpdatedAtUTC),
			})
			.ToList());

	// Get list of AppViews by UserId and return gRPC models
	public async Task<IList<AppView>> GetAppViewsByUserIdAsync(string userId)
	{
		var user = _httpContextAccessor.HttpContext?.User;
		return await _listAppViewsQuery(_dbContext, userId);
	}

	// Get AppView by Id and return gRPC model
	public async Task<AppView> GetAppViewByIdAsync(long id)
	{
		var entity = await _dbContext.AppViews
			.AsNoTracking()
			.FirstAsync(x => x.Id == id);

		return _appViewMapper.ToGrpcAppView(entity);
	}

	// Create a new AppView and return the gRPC model
	public async Task<AppView> CreateAppViewAsync(AppView view)
	{
		var entity = _appViewMapper.ToAppViewEntity(view);
		entity.CreatedAtUTC = DateTime.UtcNow;
		entity.UpdatedAtUTC = DateTime.UtcNow;

		_dbContext.AppViews.Add(entity);
		await _dbContext.SaveChangesAsync();

		return _appViewMapper.ToGrpcAppView(entity);
	}

	// Update an existing AppView and return the gRPC model
	public async Task<AppView> UpdateAppViewAsync(AppView view)
	{
		var entity = await _dbContext.AppViews
			.FirstAsync(x => x.Id == view.Id);

		entity.Name = view.Name;
		entity.Content = view.Content.ToByteArray();
		entity.UpdatedAtUTC = DateTime.UtcNow;

		await _dbContext.SaveChangesAsync();

		return _appViewMapper.ToGrpcAppView(entity);
	}

	// Delete AppView by Id and return a boolean for success
	public async Task DeleteAppViewByIdAsync(long id)
	{
		await _dbContext.AppViews
			.Where(x => x.Id == id)
			.ExecuteDeleteAsync();
	}
}
