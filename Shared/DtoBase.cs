using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public abstract class DtoBase
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
