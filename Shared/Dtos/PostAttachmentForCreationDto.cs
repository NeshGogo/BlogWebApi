using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos
{
    public class PostAttachmentForCreationDto
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
