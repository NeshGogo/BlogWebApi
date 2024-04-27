using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class PostForCreationDto
    {
        [MaxLength(150), Required]
        public string? Description { get; set; }
        public ICollection<PostAttachmentForCreationDto> PostAttachments { get; set; }
    }
}
