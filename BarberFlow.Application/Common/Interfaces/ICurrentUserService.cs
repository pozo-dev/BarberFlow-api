namespace BarberFlow.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        Guid ProfileId { get; }
        int RoleId { get; }
    }
}
