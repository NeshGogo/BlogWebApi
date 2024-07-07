
namespace Shared.Dtos
{
    public class PostAttachmentDto
    {
        public Guid PostId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
