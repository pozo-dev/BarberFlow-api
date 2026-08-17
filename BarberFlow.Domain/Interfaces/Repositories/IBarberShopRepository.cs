using BarberFlow.Domain.Entities;

public interface IBarberShopRepository
{
    Task<BarberShop?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<BarberShop?> GetByOwnerUserIdAsync(
            Guid ownerUserId,
            CancellationToken cancellationToken);

    Task<IReadOnlyList<BarberShop>> SearchActiveAsync(
        string? search,
        string? city,
        int skip,
        int take,
        CancellationToken cancellationToken);

    Task<int> CountActiveAsync(
        string? search,
        string? city,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<string>> GetActiveCitiesAsync(
        CancellationToken cancellationToken);

    Task<BarberShop?> GetActiveWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken);

    void Add(
        BarberShop barberShop);

    void Update(
        BarberShop barberShop);
}
