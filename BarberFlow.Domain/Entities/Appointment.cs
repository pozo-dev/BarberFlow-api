using BarberFlow.Domain.Enums;

namespace BarberFlow.Domain.Entities;
public class Appointment
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid BranchId { get; private set; }
    public Guid CollaboratorId { get; private set; }
    public DateTimeOffset StartDateTime { get; private set; }
    public DateTimeOffset EndDateTime { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public Guid? RescheduledToAppointmentId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public User User { get; private set; }
    public Branch Branch { get; private set; } = null!;
    public Collaborator Collaborator { get; private set; } = null!;
    public ICollection<AppointmentService> AppointmentServices { get; private set; }
    public ICollection<AppointmentActivity> Activities { get; private set; }

    private Appointment()
    {
        AppointmentServices = new List<AppointmentService>();
        Activities = new List<AppointmentActivity>();
    }

    public Appointment(Guid userId, Guid branchId, Guid collaboratorId, DateTimeOffset start, DateTimeOffset end)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        BranchId = branchId;
        CollaboratorId = collaboratorId;
        StartDateTime = start;
        EndDateTime = end;
        Status = AppointmentStatus.Scheduled;
        CreatedAt = DateTimeOffset.UtcNow;
        AppointmentServices = new List<AppointmentService>();
        Activities = new List<AppointmentActivity>();
    }

    public void Completed() => Status = AppointmentStatus.Completed;
    public void Cancel() => Status = AppointmentStatus.Cancelled;
    public void NoShow() => Status = AppointmentStatus.NoShow;
    public void RescheduleTo(Guid replacementAppointmentId)
    {
        if (!CanBeCancelled())
            throw new InvalidOperationException("Esta cita ya no puede reprogramarse.");

        Status = AppointmentStatus.Rescheduled;
        RescheduledToAppointmentId = replacementAppointmentId;
    }

    public bool CanBeCancelled()
    {
        var isInFuture = StartDateTime > DateTimeOffset.UtcNow;
        return Status == AppointmentStatus.Scheduled && isInFuture;
    }

    public void AddServices(List<Service> services, Dictionary<Guid, decimal> prices)
    {
        foreach (var service in services)
        {
            var price = prices[service.Id];

            var appointmentService = new AppointmentService(Id, service.Id, price);

            AppointmentServices.Add(appointmentService);
        }
    }
}
