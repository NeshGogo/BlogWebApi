namespace Shared.Dtos
{
    public class UserFollowingDto
    {
        public Guid UserId { get; set; }
        public Guid FollowingUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserForBasicInfoDto FollowingUser { get; set; }
    }
}
