namespace Domain.Entities
{
    public class UserImage : EntityBase
    {
        public Guid UserId { get; set; }
        public byte[] Data { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
    }
}