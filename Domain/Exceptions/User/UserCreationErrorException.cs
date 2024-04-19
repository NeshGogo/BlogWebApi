using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.User
{
    public sealed class UserCreationErrorException : BadRequestException
    {
        public UserCreationErrorException(string message) 
            : base(message)
        {
        }
    }
}
