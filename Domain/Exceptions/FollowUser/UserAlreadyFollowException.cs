namespace Domain.Exceptions.FollowUser
{
    public class UserAlreadyFollowException : BadRequestException
    {
        public UserAlreadyFollowException(Guid userId) 
            : base($"The user with the id {userId} is already follow.")
        {
        }
    }
}
