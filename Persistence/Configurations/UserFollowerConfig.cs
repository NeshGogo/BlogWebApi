using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal sealed class UserFollowerConfig : IEntityTypeConfiguration<UserFollower>
    {
        public void Configure(EntityTypeBuilder<UserFollower> builder)
        {
            builder.ToTable("UserFollowers");
            builder.HasKey(t => new { t.UserId, t.FollowerUserId});
            builder.HasOne(p => p.User)
                .WithMany(p => p.UserFollowers)
                .HasForeignKey(p => p.FollowerUserId);
        }
    }
}
