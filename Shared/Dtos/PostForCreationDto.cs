﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos
{
    public class PostForCreationDto
    {
        [MaxLength(150)]
        public string? Description { get; set; }
        [Required]
        public ICollection<IFormFile> Files { get; set; }
    }
}
