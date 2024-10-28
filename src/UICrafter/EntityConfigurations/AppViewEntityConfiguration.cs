namespace UICrafter.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UICrafter.Models;

public class AppViewEntityConfiguration : IEntityTypeConfiguration<AppViewEntity>
{
	public void Configure(EntityTypeBuilder<AppViewEntity> builder)
	{
		builder.HasKey(e => e.Id);

		builder.Property(e => e.Id)
			.UseIdentityColumn();

		builder.Property(e => e.UserId)
			.IsRequired()
			.HasColumnType("uuid");

		builder.Property(e => e.Name)
			.IsRequired()
			.HasMaxLength(255);

		builder.Property(e => e.Content)
			.IsRequired();

		builder.Property(e => e.CreatedAtUTC)
			.IsRequired();

		builder.Property(e => e.UpdatedAtUTC)
			.IsRequired();

		builder.Property(e => e.IsPublic)
			.HasDefaultValue(false);

		builder.HasIndex(e => e.UserId)
			.HasDatabaseName("IX_AppViewEntity_UserId");
	}
}
