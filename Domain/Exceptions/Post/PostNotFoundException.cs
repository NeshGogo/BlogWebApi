namespace Domain.Exceptions.Post
{
    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException(Guid postId) 
            : base($"The post with the identifier {postId} was not found.")
        {
        }
    }
}
