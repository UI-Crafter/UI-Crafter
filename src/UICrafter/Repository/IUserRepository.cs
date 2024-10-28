namespace UICrafter.Repository;

using UICrafter.Models;

public interface IUserRepository
{
	Task<UserEntity> UpsertUserEntity(UserEntity user);
}
