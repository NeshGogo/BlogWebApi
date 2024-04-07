using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IRepositoryManager
    {
        IRepository<User> UserRepo { get; }
        IRepository<Post> PostRepo { get; }
        IRepository<Post> CommentRepo { get; }
        IUnitOfWork UnitOfWork { get; }
    }
}
