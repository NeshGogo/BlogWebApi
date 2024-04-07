namespace Domain.Entities
{
    public class PostLike : EntityBase
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}