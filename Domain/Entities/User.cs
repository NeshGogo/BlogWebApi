namespace Domain.Entities
{
    public class User 
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Bio { get; set; }
        public Guid? UserImageId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime UpdatedBy { get; set; }

        public UserImage? UserImage { get; set; }
        public ICollection<UserFollower> UserFollowers { get; set; }
        public ICollection<UserFollowing> UserFollowings { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
