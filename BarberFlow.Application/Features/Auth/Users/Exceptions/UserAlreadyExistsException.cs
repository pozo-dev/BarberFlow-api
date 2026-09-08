namespace BarberFlow.Application.Features.Users.Exceptions;
public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException()
        : base("A user with this phone number already exists")
    {
    }
}
