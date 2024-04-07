namespace Domain.Entities
{
    public class Post : EntityBase
    {
        public Guid UserId { get; set; }
        public string? Description { get; set; }

        public ICollection<PostAttachment> PostAttachments { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<PostLike> PostLikes { get; set; }
    }
}