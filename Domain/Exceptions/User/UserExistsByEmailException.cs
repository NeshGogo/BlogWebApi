namespace Domain.Exceptions.User
{
    public sealed class UserExistsByEmailException : BadRequestException
    {
        public UserExistsByEmailException(string email)
            : base($"The user with the email {email} already exists.")
        {            
        }
    }
}
