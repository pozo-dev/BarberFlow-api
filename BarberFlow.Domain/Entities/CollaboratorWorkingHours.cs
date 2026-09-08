using BarberFlow.Domain.Enums;

namespace BarberFlow.Domain.Entities;

public sealed class CollaboratorWorkingHours
{
    public Guid CollaboratorId { get; private set; }
    public bool UseBranchHours { get; private set; }
    public ICollection<CollaboratorWorkPeriod> Periods { get; private set; } = new List<CollaboratorWorkPeriod>();

    private CollaboratorWorkingHours() { }

    public CollaboratorWorkingHours(Guid collaboratorId, bool useBranchHours, IEnumerable<CollaboratorWorkPeriod> periods)
    {
        if (collaboratorId == Guid.Empty) throw new ArgumentException("El colaborador es requerido.");
        CollaboratorId = collaboratorId;
        Replace(useBranchHours, periods);
    }

    public void Replace(bool useBranchHours, IEnumerable<CollaboratorWorkPeriod> periods)
    {
        var items = periods.OrderBy(x => x.DayOfWeek).ThenBy(x => x.StartTime).ToList();
        if (items.Count > 28) throw new ArgumentException("Solo se permiten cuatro tramos por día.");
        if (useBranchHours && items.Count != 0)
            throw new ArgumentException("Al usar el horario de sucursal no se envían tramos individuales.");
        foreach (var day in items.GroupBy(x => x.DayOfWeek))
        {
            if (day.Count() > 4) throw new ArgumentException("Solo se permiten cuatro tramos por día.");
            var ordered = day.ToList();
            for (var i = 1; i < ordered.Count; i++)
                if (ordered[i].StartTime < ordered[i - 1].EndTime)
                    throw new ArgumentException("Los tramos de un día no pueden superponerse.");
        }
        UseBranchHours = useBranchHours;
        Periods.Clear();
        foreach (var item in items) Periods.Add(item);
    }

    public bool Covers(DateTime localStart, DateTime localEnd)
    {
        if (localEnd <= localStart || localStart.Date != localEnd.Date) return false;
        if (UseBranchHours) return true;
        // A service may span adjacent periods, but never an actual break.
        var cursor = TimeOnly.FromDateTime(localStart);
        var end = TimeOnly.FromDateTime(localEnd);
        foreach (var period in Periods
            .Where(x => x.DayOfWeek == localStart.DayOfWeek.ToScheduleDay())
            .OrderBy(x => x.StartTime))
        {
            if (period.EndTime <= cursor) continue;
            if (period.StartTime > cursor) return false;
            cursor = period.EndTime;
            if (cursor >= end) return true;
        }
        return false;
    }
}
