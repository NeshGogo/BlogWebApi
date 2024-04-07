namespace Domain.Entities
{
    public class UserFollowing : EntityBase
    {
        public Guid UserId { get; set; }
        public Guid FollowingUserId { get; set; }

        public User FollowingUser { get; set; }
    }
}