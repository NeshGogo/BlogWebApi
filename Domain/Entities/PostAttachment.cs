namespace Domain.Entities
{
    public class PostAttachment : EntityBase
    {
        public Guid PostId { get; set; }
        public byte[] Data { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
    }
}