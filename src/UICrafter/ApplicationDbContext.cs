namespace UICrafter;

using Microsoft.EntityFrameworkCore;
using UICrafter.Models;

public class ApplicationDbContext : DbContext
{

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

	public DbSet<AppViewEntity> AppViews { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<AppViewEntity>(entity =>
		{
			entity.HasKey(e => e.Id);

			entity.Property(e => e.UserId)
				  .IsRequired()
				  .HasMaxLength(100);

			entity.Property(e => e.Name)
				  .IsRequired()
				  .HasMaxLength(255);

			entity.Property(e => e.Content)
				  .IsRequired();

			entity.Property(e => e.CreatedAtUTC)
				.IsRequired();

			entity.Property(e => e.UpdatedAtURC)
			.IsRequired();

			entity.HasIndex(e => e.UserId)
			  .HasDatabaseName("IX_AppViewEntity_UserId");
		});
	}
}
