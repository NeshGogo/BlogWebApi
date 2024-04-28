using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IServiceManager
    {
        IUserService UserService { get; }
        IPostService PostService { get; }
        ICommentService CommentService { get; }
    }
}
