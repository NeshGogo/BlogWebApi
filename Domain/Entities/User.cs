using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Bio { get; set; }
        public Guid? UserImageId { get; set; }
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
