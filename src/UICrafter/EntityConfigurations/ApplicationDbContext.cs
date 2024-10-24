namespace UICrafter.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using UICrafter.Models;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

	public DbSet<AppViewEntity> AppViews { get; set; }
	public DbSet<UserEntity> Users { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new AppViewEntityConfiguration());
		modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
	}
}
