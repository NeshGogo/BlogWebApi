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

        public string BuildConfirmEmailBody(string host, string token)
        {
            var url = new Uri($"{host}/api/Accounts/ConfirmEmail?token={Uri.EscapeDataString(token)}&userId={Uri.EscapeDataString(Id.ToString())}");
            return @$"
                <h2>Hi {Name},</h2>
                <p>
                    Thanks for registering in blob post we are thrilled to have you here. 
                    There is a last step that we need you to take and is to confirm your email in the link below.
                </p>                
                <p>
                    <a href=""{url.AbsoluteUri}"">Confirm email!</a>        
                </p>
                </br>
                <p>Regards,</p>
                </br>
                <p>Blog Post</p>
            ";
        }
    }
}
