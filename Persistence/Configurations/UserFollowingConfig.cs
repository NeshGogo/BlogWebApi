
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal class UserFollowingConfig : IEntityTypeConfiguration<UserFollowing>
    {
        public void Configure(EntityTypeBuilder<UserFollowing> builder)
        {
            builder.ToTable("UserFollowings");
            builder.HasKey(t => new { t.UserId, t.FollowingUserId });
            builder.HasOne(p => p.User)
                .WithMany(p => p.UserFollowings)
                .HasForeignKey(p => p.FollowingUserId);
        }
    }
}
