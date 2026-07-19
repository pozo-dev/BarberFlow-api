using BarberFlow.Domain.Entities;

public interface IBarberShopRepository
{
    Task<BarberShop?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<BarberShop?> GetByOwnerUserIdAsync(
            Guid ownerUserId,
            CancellationToken cancellationToken);

    void Add(
        BarberShop barberShop);

    void Update(
        BarberShop barberShop);
}