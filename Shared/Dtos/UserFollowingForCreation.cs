using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos
{
    public class UserFollowingForCreation
    {
        [Required]
        public Guid FollowingUserId { get; set; }
    }
}
