namespace Domain.Exceptions.User
{
    public class UserLoginByEmailOrPasswordException : BadRequestException
    {
        public UserLoginByEmailOrPasswordException() 
            : base("The email or password is wrong!")
        {
        }
    }
}
