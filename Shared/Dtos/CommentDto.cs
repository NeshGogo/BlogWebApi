namespace Shared.Dtos
{
    public class CommentDto : DtoBase
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string Content { get; set; }

        public UserDto User { get; set; }
    }
}
