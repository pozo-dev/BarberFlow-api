namespace BarberFlow.Domain.Entities;
public class Collaborator
{
    public Guid Id { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid? UserProfileId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public Branch Branch { get; private set; } = null!;
    public UserProfile? UserProfile { get; private set; }

    private Collaborator() { }

    private Collaborator(Guid branchId, string fullName, string phoneNumber)
    {
        Id = Guid.NewGuid();
        BranchId = branchId;
        FullName = fullName.Trim();
        PhoneNumber = phoneNumber.Trim();
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Collaborator Create(Guid branchId, string fullName, string phoneNumber)
    {
        if (branchId == Guid.Empty) throw new ArgumentException("La sucursal es requerida.");
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("El nombre completo es requerido.");
        if (string.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentException("El teléfono es requerido.");

        return new Collaborator(branchId, fullName, phoneNumber);
    }

    public void Update(Guid branchId, string fullName, string phoneNumber)
    {
        if (branchId == Guid.Empty) throw new ArgumentException("La sucursal es requerida.");
        if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("El nombre completo es requerido.");
        if (string.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentException("El teléfono es requerido.");

        BranchId = branchId;
        FullName = fullName.Trim();
        PhoneNumber = phoneNumber.Trim();
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

    public void LinkUserProfile(Guid userProfileId)
    {
        if (userProfileId == Guid.Empty)
            throw new ArgumentException("El perfil de Barbero es requerido.");

        UserProfileId = userProfileId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
