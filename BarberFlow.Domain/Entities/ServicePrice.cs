namespace BarberFlow.Domain.Entities;
public class ServicePrice
{
    public Guid Id { get; private set; }
    public Guid ServiceId { get; private set; }
    public decimal Price { get; private set; }
    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsCurrent { get; private set; }

    public Service Service { get; private set; } = null!;

    private ServicePrice() { } // EF

    public static ServicePrice Create(Guid serviceId, decimal price, DateTimeOffset effectiveFrom)
    {
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.");

        if (effectiveFrom == default)
            throw new ArgumentException("Effective date is required.");

        return new ServicePrice(serviceId, price, effectiveFrom);
    }

    private ServicePrice(Guid serviceId, decimal price, DateTimeOffset effectiveFrom)
    {
        Id = Guid.NewGuid();
        ServiceId = serviceId;
        Price = price;
        EffectiveFrom = effectiveFrom;
        CreatedAt = DateTimeOffset.UtcNow;
        IsCurrent = true;
    }

    public void Deactivate()
    {
        IsCurrent = false;
    }

    public void Activate()
    {
        IsCurrent = true;
    }
}
