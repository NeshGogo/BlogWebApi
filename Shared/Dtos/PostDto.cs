﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class PostDto : DtoBase
    {
        public Guid UserId { get; set; }
        public string? Description { get; set; }
        public ICollection<PostAttachmentDto> PostAttachments { get; set; }
    }
}
