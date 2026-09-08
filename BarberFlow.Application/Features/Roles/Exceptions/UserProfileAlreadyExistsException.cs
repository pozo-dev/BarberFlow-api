namespace BarberFlow.Application.Features.Roles.Exceptions;
public class UserProfileAlreadyExistsException : Exception
{
    public UserProfileAlreadyExistsException()
         : base("The user already has this profile.")
    {
    }
}
