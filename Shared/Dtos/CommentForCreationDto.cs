using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos
{
    public class CommentForCreationDto
    {
        [Required, MaxLength(150)]
        public string Content { get; set; }
    }
}
