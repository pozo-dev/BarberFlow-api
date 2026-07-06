using BarberFlow.Domain.Entities;

public interface IBarberShopRepository
{
    Task<BarberShop?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}