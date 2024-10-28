namespace UICrafter.Repository;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UICrafter.EntityConfigurations;
using UICrafter.Models;

public class UserRepository : IUserRepository
{
	private readonly ApplicationDbContext _dbContext;

	public UserRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

	public async Task<UserEntity> UpsertUserEntity(UserEntity user)
	{
		if (user.Id == Guid.Empty)
		{
			throw new ArgumentException("User Id cannot be the default GUID (00000000-0000-0000-0000-000000000000)");
		}

		var affectedRows = await _dbContext.Users
			.Where(u => u.Id == user.Id)
			.ExecuteUpdateAsync(u => u
				.SetProperty(x => x.Name, user.Name)
				.SetProperty(x => x.Email, user.Email)
			);

		if (affectedRows == 0)
		{
			_dbContext.Users.Add(user);
			await _dbContext.SaveChangesAsync();
		}

		return user;
	}
}
