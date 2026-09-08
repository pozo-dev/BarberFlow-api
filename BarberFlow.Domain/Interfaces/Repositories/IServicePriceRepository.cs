namespace BarberFlow.Domain.Interfaces.Repositories;
public interface IServicePriceRepository
{
    Task<Dictionary<Guid, decimal>> GetCurrentPricesAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken);
}
