using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class UserFollowing
    {
        public Guid UserId { get; set; }
        public Guid FollowingUserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [MaxLength(200)]
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        [MaxLength(200)]
        public string? UpdatedBy { get; set; }

        public User User { get; set; }
    }
}