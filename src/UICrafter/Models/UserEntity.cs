namespace UICrafter.Models;

public class UserEntity
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public required string Email { get; set; }
}
