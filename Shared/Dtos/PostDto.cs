namespace Shared.Dtos
{
    public class PostDto : DtoBase
    {
        public Guid UserId { get; set; }
        public string? Description { get; set; }
        public bool Liked { get; set; }
        public int AmountOfComments { get; set; }
        public int AmountOfLikes { get; set; }
        public ICollection<PostAttachmentDto> PostAttachments { get; set; }
        public UserDto? User { get; set; }
    }
}
