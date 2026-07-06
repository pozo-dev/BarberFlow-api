namespace BarberFlow.Application.Features.Roles.Exceptions
{
    public class RoleNotFoundException : Exception
    {
        public RoleNotFoundException()
            : base("Role not found.")
        {
        }
    }
}
