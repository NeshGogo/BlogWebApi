using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.Post
{
    public class PostDoesNotBelongToUserException : BadRequestException
    {
        public PostDoesNotBelongToUserException(Guid postId) 
            : base($"The post with the identifier {postId} does not belong to the logged in user")
        {
        }
    }
}
