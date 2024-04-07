namespace Domain.Exceptions.User
{
    public sealed class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(Guid userId) 
            : base($"The user with the identifier {userId} was not found.")
        {
            
        }
    }
}
