using BarberFlow.Application.Common.Time;
using BarberFlow.Application.Features.Appointments.Availability;
using BarberFlow.Application.Features.Client.Appointments.Queries.GetAppointmentAvailability;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces.Repositories;
using BarberFlow.Infrastructure.Persistence;
using BarberFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BarberFlow.Tests;

public sealed class AvailabilityTests
{
    private const string Zone = "America/Managua";
    private static readonly DateOnly Day = new(2035, 1, 1);
    private static readonly ScheduleDay Weekday = Day.DayOfWeek.ToScheduleDay();
    private static DateTimeOffset At(int hour, int minute = 0) => BranchTimeZone.ToUtc(Day, new TimeOnly(hour, minute), Zone);
    private static CollaboratorWorkPeriod Period(int start, int end) => new(Weekday, new TimeOnly(start, 0), new TimeOnly(end, 0));
    private static Collaborator Candidate() => Collaborator.Create(Guid.NewGuid(), "Professional", "88888888");

    [Fact]
    public async Task NoCandidatesReturnsNoSlots()
    {
        var availability = await ProfessionalAvailability.LoadAsync(new FakeRepository(), [], At(8), At(18), default);
        Assert.Empty(AppointmentAvailabilityRules.GetAvailableSlots(Day, new(8, 0), new(18, 0), TimeSpan.FromMinutes(30), [], availability, Zone));
    }

    [Fact]
    public async Task ExistingProfessionalInheritsBranchHours()
    {
        var candidate = Candidate();
        var availability = await Load(new FakeRepository(), candidate);
        var slots = AppointmentAvailabilityRules.GetAvailableSlots(Day, new(8, 0), new(18, 0), TimeSpan.FromMinutes(75), [candidate], availability, Zone);
        Assert.Equal(new TimeOnly(8, 0), slots.First().DisplayTime);
        Assert.Equal(new TimeOnly(16, 30), slots.Last().DisplayTime);
    }

    [Fact]
    public async Task EntireServiceMustFitInsideWorkPeriodWithoutCrossingBreak()
    {
        var candidate = Candidate();
        var repository = new FakeRepository();
        repository.Hours[candidate.Id] = new(candidate.Id, false, [Period(9, 12), Period(13, 17)]);
        var availability = await Load(repository, candidate);
        Assert.True(availability.CanAttend(candidate.Id, At(10, 45), At(12), Zone));
        Assert.False(availability.CanAttend(candidate.Id, At(11), At(12, 15), Zone));
        Assert.False(availability.CanAttend(candidate.Id, At(12), At(13), Zone));
        Assert.True(availability.CanAttend(candidate.Id, At(13), At(14, 15), Zone));
        Assert.False(availability.CanAttend(candidate.Id, At(16), At(17, 15), Zone));
    }

    [Fact]
    public async Task IndividualHoursNeverExtendBranchOpeningHours()
    {
        var candidate = Candidate();
        var repository = new FakeRepository();
        repository.Hours[candidate.Id] = new(candidate.Id, false, [Period(6, 23)]);
        var availability = await Load(repository, candidate);
        var slots = AppointmentAvailabilityRules.GetAvailableSlots(Day, new(9, 0), new(12, 0), TimeSpan.FromHours(1), [candidate], availability, Zone);
        Assert.Equal(5, slots.Count);
        Assert.Equal(new TimeOnly(9, 0), slots.First().DisplayTime);
        Assert.Equal(new TimeOnly(11, 0), slots.Last().DisplayTime);
    }

    [Fact]
    public async Task EmptyIndividualWeekHasNoAvailability()
    {
        var candidate = Candidate();
        var repository = new FakeRepository();
        repository.Hours[candidate.Id] = new(candidate.Id, false, []);
        var availability = await Load(repository, candidate);
        Assert.Empty(AppointmentAvailabilityRules.GetAvailableSlots(Day, new(8, 0), new(18, 0), TimeSpan.FromMinutes(30), [candidate], availability, Zone));
    }

    [Fact]
    public async Task AbsenceUsesHalfOpenIntervals()
    {
        var candidate = Candidate();
        var repository = new FakeRepository();
        repository.TimeOff.Add(new(candidate.Id, CollaboratorTimeOffType.Absence, At(10), At(11)));
        var availability = await Load(repository, candidate);
        Assert.True(availability.CanAttend(candidate.Id, At(9), At(10), Zone));
        Assert.False(availability.CanAttend(candidate.Id, At(9, 30), At(10, 30), Zone));
        Assert.True(availability.CanAttend(candidate.Id, At(11), At(12), Zone));
    }

    [Fact]
    public async Task VacationBlocksEveryIncludedDay()
    {
        var candidate = Candidate();
        var repository = new FakeRepository();
        var vacation = new CollaboratorTimeOff(
            candidate.Id,
            CollaboratorTimeOffType.Vacation,
            BranchTimeZone.ToUtc(Day, Zone),
            BranchTimeZone.ToUtc(Day.AddDays(3), Zone),
            true);
        vacation.Decide(AvailabilityChangeStatus.Approved, Guid.NewGuid());
        repository.TimeOff.Add(vacation);
        var availability = await ProfessionalAvailability.LoadAsync(repository, [candidate.Id],
            BranchTimeZone.ToUtc(Day, Zone), BranchTimeZone.ToUtc(Day.AddDays(4), Zone), default);
        Assert.False(availability.CanAttend(candidate.Id, At(9), At(10), Zone));
        Assert.False(availability.CanAttend(candidate.Id, At(9).AddDays(2), At(10).AddDays(2), Zone));
        Assert.True(availability.CanAttend(candidate.Id, At(9).AddDays(3), At(10).AddDays(3), Zone));
    }

    [Fact]
    public async Task PendingVacationDoesNotBlockUntilOwnerApprovesIt()
    {
        var candidate = Candidate();
        var repository = new FakeRepository();
        repository.TimeOff.Add(new CollaboratorTimeOff(
            candidate.Id,
            CollaboratorTimeOffType.Vacation,
            BranchTimeZone.ToUtc(Day, Zone),
            BranchTimeZone.ToUtc(Day.AddDays(1), Zone),
            true));
        var availability = await Load(repository, candidate);
        Assert.True(availability.CanAttend(candidate.Id, At(9), At(10), Zone));
    }

    [Fact]
    public async Task AnyoneAvailableSkipsAbsentProfessional()
    {
        var absent = Candidate();
        var available = Candidate();
        var repository = new FakeRepository();
        repository.TimeOff.Add(new(absent.Id, CollaboratorTimeOffType.Absence, At(8), At(18)));
        var snapshot = await ProfessionalAvailability.LoadAsync(repository, [absent.Id, available.Id], At(8), At(18), default);
        var slots = AppointmentAvailabilityRules.GetAvailableSlots(Day, new(8, 0), new(18, 0), TimeSpan.FromMinutes(75), [absent, available], snapshot, Zone);
        Assert.NotEmpty(slots);
        Assert.All(slots, x => Assert.Equal(available.Id, x.ProfessionalId));
    }

    [Fact]
    public async Task ExistingAppointmentsBlockCandidateButReschedulingCanExcludeOriginal()
    {
        var candidate = Candidate();
        var appointmentId = Guid.NewGuid();
        var repository = new FakeRepository();
        repository.Booked.Add(new(candidate.Id, appointmentId, At(9), At(10)));
        Assert.False((await Load(repository, candidate)).CanAttend(candidate.Id, At(9), At(10), Zone));
        var snapshot = await ProfessionalAvailability.LoadAsync(repository, [candidate.Id], At(8), At(18), default, appointmentId);
        Assert.True(snapshot.CanAttend(candidate.Id, At(9), At(10), Zone));
    }

    [Fact]
    public async Task LongDurationDoesNotWrapIntoFollowingDay()
    {
        var candidate = Candidate();
        Assert.Empty(AppointmentAvailabilityRules.GetAvailableSlots(Day, new(8, 0), new(18, 0),
            TimeSpan.FromHours(25), [candidate], await Load(new FakeRepository(), candidate), Zone));
    }

    [Fact]
    public void OverlappingPeriodsAndInvalidDaysAreRejected()
    {
        Assert.Throws<ArgumentException>(() => new CollaboratorWorkingHours(Guid.NewGuid(), false, [Period(8, 12), Period(11, 15)]));
        Assert.Throws<ArgumentException>(() => new CollaboratorWorkPeriod((ScheduleDay)0, new(8, 0), new(12, 0)));
        Assert.Throws<ArgumentException>(() => new CollaboratorWorkPeriod(Weekday, new(12, 0), new(8, 0)));
    }

    [Fact]
    public void AdjacentPeriodsDoNotCreateAnArtificialBreak()
    {
        var hours = new CollaboratorWorkingHours(Guid.NewGuid(), false, [Period(8, 12), Period(12, 17)]);
        Assert.True(hours.Covers(Day.ToDateTime(new(11, 30)), Day.ToDateTime(new(12, 30))));
    }

    [Fact]
    public void EditingPersistedPeriodsDeletesOldRowsAndInsertsNewRows()
    {
        using var context = new BarberFlowDbContext(new DbContextOptionsBuilder<BarberFlowDbContext>()
            .UseSqlServer("Server=(local);Database=Unused;Integrated Security=true;TrustServerCertificate=true").Options);
        var hours = new CollaboratorWorkingHours(Guid.NewGuid(), false, [Period(8, 12)]);
        var old = hours.Periods.Single();
        context.Attach(hours);
        new CollaboratorAvailabilityRepository(context).ReplaceWorkingHours(hours, false, [Period(9, 13)]);
        context.ChangeTracker.DetectChanges();
        Assert.Equal(EntityState.Deleted, context.Entry(old).State);
        Assert.Equal(EntityState.Added, context.Entry(hours.Periods.Single()).State);
    }

    private static Task<ProfessionalAvailability> Load(FakeRepository repository, Collaborator candidate) =>
        ProfessionalAvailability.LoadAsync(repository, [candidate.Id], At(0), At(0).AddDays(1), default);

    private sealed class FakeRepository : ICollaboratorAvailabilityRepository
    {
        public Dictionary<Guid, CollaboratorWorkingHours> Hours { get; } = [];
        public List<CollaboratorTimeOff> TimeOff { get; } = [];
        public List<ProfessionalBusyInterval> Booked { get; } = [];
        public Task<IReadOnlyDictionary<Guid, CollaboratorWorkingHours>> GetWorkingHoursAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct) => Task.FromResult<IReadOnlyDictionary<Guid, CollaboratorWorkingHours>>(Hours);
        public Task<List<CollaboratorTimeOff>> GetTimeOffAsync(IReadOnlyCollection<Guid> ids, DateTimeOffset start, DateTimeOffset end, CancellationToken ct) => Task.FromResult(TimeOff);
        public Task<List<ProfessionalBusyInterval>> GetBusyIntervalsAsync(IReadOnlyCollection<Guid> ids, DateTimeOffset start, DateTimeOffset end, CancellationToken ct) => Task.FromResult(Booked);
        public Task<CollaboratorWorkingHours?> GetWorkingHoursAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<List<CollaboratorTimeOff>> GetTimeOffAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public Task<List<Appointment>> GetFutureAppointmentsAsync(Guid id, CancellationToken ct) => throw new NotSupportedException();
        public void Add(CollaboratorWorkingHours hours) => throw new NotSupportedException();
        public void ReplaceWorkingHours(CollaboratorWorkingHours current, bool useBranchHours, IEnumerable<CollaboratorWorkPeriod> periods) => throw new NotSupportedException();
        public void Add(CollaboratorTimeOff timeOff) => throw new NotSupportedException();
        public void Remove(CollaboratorTimeOff timeOff) => throw new NotSupportedException();
        public Task<IAvailabilityMutation> BeginMutationAsync(IReadOnlyCollection<Guid> ids, CancellationToken ct) => throw new NotSupportedException();
    }
}
