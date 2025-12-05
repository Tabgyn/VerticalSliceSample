using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VerticalSliceSample.Api.Database.Entities;

namespace VerticalSliceSample.Api.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(m => m.Id);

        builder.HasIndex(x => x.Username)
            .IsUnique();

        builder.HasIndex(x => x.ReferenceId)
            .IsUnique();
    }
}
