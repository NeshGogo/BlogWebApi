namespace Domain.Entities
{
    public class Comment : EntityBase
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; }
        
        public User User { get; set; }
    }
}