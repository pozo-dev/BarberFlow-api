public class UserProfileNotFoundException : Exception
{
    public UserProfileNotFoundException()
        : base("User profile not found.")
    {
    }
}