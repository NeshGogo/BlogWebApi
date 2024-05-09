namespace Domain.Exceptions.FollowUser
{
    public class UserCannotFollowThemselvesException : BadRequestException
    {
        public UserCannotFollowThemselvesException() 
            : base("Users can not follow themselves, the userId has to be distinct")
        {
        }
    }
}
