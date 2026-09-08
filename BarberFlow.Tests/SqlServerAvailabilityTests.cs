using BarberFlow.Application.Common.Time;
using BarberFlow.Application.Features.Appointments.Availability;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Enums;
using BarberFlow.Infrastructure.Persistence;
using BarberFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BarberFlow.Tests;

public sealed class SqlServerAvailabilityTests
{
    [SqlServerFact]
    public async Task AvailabilityPersistsAndQueriesLinkedProfessionalsWithinRolledBackTransaction()
    {
        var options = new DbContextOptionsBuilder<BarberFlowDbContext>()
            .UseSqlServer(Environment.GetEnvironmentVariable("BARBERFLOW_TEST_CONNECTION")).Options;
        await using var context = new BarberFlowDbContext(options);
        var branchId = await context.Branches.Select(x => x.Id).FirstAsync();
        var userId = await context.Users.Select(x => x.Id).FirstAsync();
        var profileId = await context.UserProfiles.Select(x => x.Id).FirstAsync();
        var lockId = await context.Collaborators.Select(x => x.Id).FirstAsync();
        var repository = new CollaboratorAvailabilityRepository(context);
        // All fixture writes are rolled back, including on assertion failure.
        await using var mutation = await repository.BeginMutationAsync([lockId], default);
        var first = Collaborator.Create(branchId, "Availability test A", "00000000");
        var second = Collaborator.Create(branchId, "Availability test B", "00000001");
        first.LinkUserProfile(profileId);
        second.LinkUserProfile(profileId);
        context.AddRange(first, second);
        await context.SaveChangesAsync();

        var day = new DateOnly(2035, 1, 1);
        DateTimeOffset At(int hour) => BranchTimeZone.ToUtc(day, new(hour, 0), "America/Managua");
        var hours = new CollaboratorWorkingHours(first.Id, false, [
            new(day.DayOfWeek.ToScheduleDay(), new(8, 0), new(12, 0)),
            new(day.DayOfWeek.ToScheduleDay(), new(13, 0), new(18, 0)),
        ]);
        repository.Add(hours);
        var absence = new CollaboratorTimeOff(first.Id, CollaboratorTimeOffType.Absence, At(15), At(16));
        repository.Add(absence);
        var appointment = new Appointment(userId, branchId, second.Id, At(9), At(10));
        context.Add(appointment);
        await context.SaveChangesAsync();

        var snapshot = await ProfessionalAvailability.LoadAsync(repository, [first.Id], At(8), At(18), default);
        Assert.False(snapshot.CanAttend(first.Id, At(9), At(10), "America/Managua"));
        Assert.False(snapshot.CanAttend(first.Id, At(12), At(13), "America/Managua"));
        Assert.False(snapshot.CanAttend(first.Id, At(15), At(16), "America/Managua"));
        Assert.True(snapshot.CanAttend(first.Id, At(10), At(11), "America/Managua"));

        appointment.Cancel();
        repository.Remove(absence);
        repository.ReplaceWorkingHours(hours, false, [new(day.DayOfWeek.ToScheduleDay(), new(8, 0), new(18, 0))]);
        await context.SaveChangesAsync();
        snapshot = await ProfessionalAvailability.LoadAsync(repository, [first.Id], At(8), At(18), default);
        Assert.True(snapshot.CanAttend(first.Id, At(9), At(10), "America/Managua"));
        Assert.True(snapshot.CanAttend(first.Id, At(12), At(13), "America/Managua"));
        Assert.True(snapshot.CanAttend(first.Id, At(15), At(16), "America/Managua"));
        Assert.Single((await repository.GetWorkingHoursAsync(first.Id, default))!.Periods);
    }
}

public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("BARBERFLOW_TEST_CONNECTION")))
            Skip = "Set BARBERFLOW_TEST_CONNECTION to run the transactional SQL Server smoke test.";
    }
}
