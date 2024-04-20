namespace Domain.Exceptions.User
{
    public sealed class UserNameAlreadyExistsException : BadRequestException
    {
        public UserNameAlreadyExistsException(string userName)
            : base($"Already exists an user with the user name \"{userName}\"")
        {
        }
    }
}
