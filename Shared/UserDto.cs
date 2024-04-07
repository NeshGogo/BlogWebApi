﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public sealed class UserDto : DtoBase
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Bio { get; set; }
        public Guid? UserImageId { get; set; }
    }
}
