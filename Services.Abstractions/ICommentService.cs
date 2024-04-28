using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetAllCommentsByPost(Guid postId, CancellationToken cancellation = default);
        Task<CommentDto> CreateComment(Guid postId, CommentForCreationDto creationDto, CancellationToken cancellation = default);
    }
}
