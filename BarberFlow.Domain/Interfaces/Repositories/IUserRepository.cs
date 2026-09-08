using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories;
public interface IUserRepository
{
    void Add(User user);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken);
}
