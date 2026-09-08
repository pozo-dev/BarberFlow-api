namespace BarberFlow.Domain.Entities;
public class Branch
{
    public const int WeeklyScheduleDays = 7;

    private static readonly TimeOnly DefaultOpenTime = new(8, 0);
    private static readonly TimeOnly DefaultCloseTime = new(18, 0);
    public Guid Id { get; private set; }

    public Guid BarberShopId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Address { get; private set; } = string.Empty;

    public int LocationSearchId { get; private set; }

    public string PhoneNumber { get; private set; } = string.Empty;

    public bool IsMain { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    // IANA identifier. It keeps commercial schedules independent from the
    // timezone configured on a customer's device or on the API host.
    public string TimeZoneId { get; private set; } = "America/Managua";

    public BarberShop BarberShop { get; private set; } = null!;

    public LocationSearch LocationSearch { get; private set; } = null!;

    public ICollection<BranchSchedule> Schedules { get; private set; }
        = new List<BranchSchedule>();

    public ICollection<BarberAssignment> BarberAssignments { get; private set; }
        = new List<BarberAssignment>();

    public ICollection<Appointment> Appointments { get; private set; }
        = new List<Appointment>();

    private Branch()
    {
    }

    private Branch(
        Guid barberShopId,
        string name,
        string address,
        int locationSearchId,
        string phoneNumber,
        bool isMain)
    {
        Id = Guid.NewGuid();

        BarberShopId = barberShopId;

        Name = name.Trim();

        Address = address.Trim();

        LocationSearchId = locationSearchId;

        PhoneNumber = phoneNumber.Trim();

        IsMain = isMain;

        IsActive = true;

        CreatedAt = DateTimeOffset.UtcNow;

        InitializeDefaultSchedules();
    }

    public static Branch Create(
        Guid barberShopId,
        string name,
        string address,
        int locationSearchId,
        string phoneNumber,
        bool isMain = false)
    {
        if (barberShopId == Guid.Empty)
            throw new ArgumentException("La barbería es requerida.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre es requerido.");

        if (name.Trim().Length > 120)
            throw new ArgumentException("El nombre no puede superar 120 caracteres.");

        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("La dirección es requerida.");

        if (address.Trim().Length > 300)
            throw new ArgumentException("La dirección no puede superar 300 caracteres.");

        if (locationSearchId <= 0)
            throw new ArgumentException("La ubicación es requerida.");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("El teléfono es requerido.");

        if (phoneNumber.Trim().Length > 20)
            throw new ArgumentException("El teléfono no puede superar 20 caracteres.");

        return new Branch(
            barberShopId,
            name,
            address,
            locationSearchId,
            phoneNumber,
            isMain);
    }

    public void Update(
        string name,
        string address,
        int locationSearchId,
        string phoneNumber)
    {
        Name = name.Trim();

        Address = address.Trim();

        if (locationSearchId <= 0)
            throw new ArgumentException("La ubicación es requerida.");

        LocationSearchId = locationSearchId;

        PhoneNumber = phoneNumber.Trim();

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateSchedules(IReadOnlyCollection<BranchScheduleUpdate> schedules)
    {
        if (schedules is null || schedules.Count != WeeklyScheduleDays)
            throw new ArgumentException("La sucursal debe tener exactamente siete horarios.");

        if (Schedules.Count != WeeklyScheduleDays ||
            Schedules.Select(x => x.DayOfWeek).Distinct().Count() != WeeklyScheduleDays)
            throw new InvalidOperationException("La sucursal no tiene una configuración semanal completa.");

        if (schedules.Select(x => x.ScheduleId).Distinct().Count() != WeeklyScheduleDays ||
            schedules.Select(x => x.DayOfWeek).Distinct().Count() != WeeklyScheduleDays)
            throw new ArgumentException("Los horarios deben contener un único registro por día.");

        var existingSchedules = Schedules.ToDictionary(x => x.Id);

        if (schedules.Any(x => !existingSchedules.TryGetValue(x.ScheduleId, out var schedule) ||
                               schedule.BranchId != Id ||
                               schedule.DayOfWeek != x.DayOfWeek))
        {
            throw new ArgumentException(
                "Todos los horarios deben pertenecer a la sucursal y conservar su día.");
        }

        foreach (var schedule in schedules)
        {
            existingSchedules[schedule.ScheduleId].Update(
                schedule.OpenTime,
                schedule.CloseTime,
                schedule.IsClosed);
        }

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private void InitializeDefaultSchedules()
    {
        foreach (var day in Enum.GetValues<BarberFlow.Domain.Enums.ScheduleDay>())
        {
            Schedules.Add(BranchSchedule.Create(
                Id,
                day,
                DefaultOpenTime,
                DefaultCloseTime));
        }
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

    public void SetAsMain()
    {
        IsMain = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RemoveMain()
    {
        IsMain = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
