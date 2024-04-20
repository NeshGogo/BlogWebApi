namespace Domain.Exceptions.User
{
    public class UserNotFoundByEmailException : NotFoundException
    {
        public UserNotFoundByEmailException(string email) 
            : base($"The user with the email {email} was not found.")
        {
        }
    }
}
