using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.User
{
    public class RefreshTokenException : BadRequestException
    {
        public RefreshTokenException() 
            : base("Invalid client request. The tokenDto has some invalid values.")
        {
        }
    }
}
