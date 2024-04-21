using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string Name { get; set; }
        public string? Bio { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [MaxLength(200)]
        public string CreatedBy { get; set; }
        public DateTime Updated { get; set; }
        [MaxLength(200)]
        public string UpdatedBy { get; set; }

        public string? UserImageUrl { get; set; }
        public ICollection<UserFollower> UserFollowers { get; set; }
        public ICollection<UserFollowing> UserFollowings { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
