using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces.Repositories;

namespace BarberFlow.Application.Features.Appointments.Availability;

// One snapshot per query, reused for every slot and every day of a calendar.
public sealed class ProfessionalAvailability
{
    private readonly IReadOnlyDictionary<Guid, CollaboratorWorkingHours> _hours;
    private readonly ILookup<Guid, CollaboratorTimeOff> _timeOff;
    private readonly ILookup<Guid, ProfessionalBusyInterval> _booked;

    private ProfessionalAvailability(IReadOnlyDictionary<Guid, CollaboratorWorkingHours> hours, IEnumerable<CollaboratorTimeOff> timeOff, IEnumerable<ProfessionalBusyInterval> booked)
    {
        _hours = hours;
        _timeOff = timeOff.Where(x => x.Status == AvailabilityChangeStatus.Approved)
            .ToLookup(x => x.CollaboratorId);
        _booked = booked.ToLookup(x => x.CollaboratorId);
    }

    public static async Task<ProfessionalAvailability> LoadAsync(
        ICollaboratorAvailabilityRepository repository, IReadOnlyCollection<Guid> ids,
        DateTimeOffset start, DateTimeOffset end, CancellationToken ct, Guid? excludedAppointmentId = null)
    {
        if (ids.Count == 0) return new ProfessionalAvailability(new Dictionary<Guid, CollaboratorWorkingHours>(), [], []);
        var hours = await repository.GetWorkingHoursAsync(ids, ct);
        var timeOff = await repository.GetTimeOffAsync(ids, start, end, ct);
        var booked = await repository.GetBusyIntervalsAsync(ids, start, end, ct);
        return new ProfessionalAvailability(hours, timeOff, booked.Where(x => x.AppointmentId != excludedAppointmentId));
    }

    public bool CanAttend(Guid collaboratorId, DateTimeOffset start, DateTimeOffset end, string timeZoneId)
    {
        if (end <= start) return false;
        var localStart = BranchTimeZone.ToBranchTime(start, timeZoneId).DateTime;
        var localEnd = BranchTimeZone.ToBranchTime(end, timeZoneId).DateTime;
        return (!_hours.TryGetValue(collaboratorId, out var hours) || hours.Covers(localStart, localEnd))
            && !_timeOff[collaboratorId].Any(x => x.Overlaps(start, end))
            && !_booked[collaboratorId].Any(x => start < x.End && end > x.Start);
    }
}
