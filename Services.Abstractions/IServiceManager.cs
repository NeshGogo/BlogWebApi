namespace Services.Abstractions;

public interface IServiceManager
{
    IUserService UserService { get; }
    IPostService PostService { get; }
    ICommentService CommentService { get; }
    IFollowService followService { get; }
    IGenerativeAiService GenerativeAiService { get; }
}
