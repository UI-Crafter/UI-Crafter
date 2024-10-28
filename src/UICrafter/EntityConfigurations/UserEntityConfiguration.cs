namespace UICrafter.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UICrafter.Models;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
	public void Configure(EntityTypeBuilder<UserEntity> builder)
	{
		builder.HasKey(e => e.Id);

		builder.Property(e => e.Name)
			.IsRequired()
			.HasMaxLength(255);

		builder.Property(e => e.Email)
			.IsRequired()
			.HasMaxLength(255);
	}
}
