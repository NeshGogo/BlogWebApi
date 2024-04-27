using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class PostForUpdateDto
    {
        [MaxLength(150)]
        public string? Description { get; set; }
    }
}
