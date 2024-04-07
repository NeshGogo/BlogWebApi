namespace Domain.Entities
{
    public class UserFollower : EntityBase
    {
        public Guid UserId { get; set; }
        public Guid FolloweUserId { get; set; }

        public User FollowerUser { get; set; }
    }
}