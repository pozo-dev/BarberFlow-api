namespace BarberFlow.Domain.Entities;
public class BarberAssignment
{
    public Guid Id { get; private set; }

    public Guid BranchId { get; private set; }
    public Guid BarberProfileId { get; private set; }

    public bool IsPrimary { get; private set; }
    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public Branch Branch { get; private set; } = null!;
    public UserProfile BarberProfile { get; private set; } = null!;

    private BarberAssignment() { }

    private BarberAssignment(
        Guid branchId,
        Guid barberProfileId,
        bool isPrimary)
    {
        Id = Guid.NewGuid();
        BranchId = branchId;
        BarberProfileId = barberProfileId;
        IsPrimary = isPrimary;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static BarberAssignment Create(
        Guid branchId,
        Guid barberProfileId,
        bool isPrimary = false)
    {
        if (branchId == Guid.Empty)
            throw new ArgumentException("La sucursal es requerida.");

        if (barberProfileId == Guid.Empty)
            throw new ArgumentException("El perfil del barbero es requerido.");

        return new BarberAssignment(
            branchId,
            barberProfileId,
            isPrimary);
    }

    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
