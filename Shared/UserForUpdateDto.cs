using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public sealed class UserForUpdateDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required, MaxLength(20)]
        public string UserName { get; set; }

        [MaxLength(150)]
        public string? Bio { get; set; }
    }
}
