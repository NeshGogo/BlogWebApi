using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public class PostFilterDto
    {
        public bool Me { get; set; } = false;
        public bool Following { get; set; } = false;
        public Guid? UserId { get; set; }
    }
}
