using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Owner.Availability;

public sealed record GetOwnerAvailabilityQuery(Guid CollaboratorId, int Year, int Month) : IRequest<OwnerAvailabilityDto>;

public sealed record OwnerAvailabilityDto(
    IReadOnlyList<OwnerAvailabilityEntryDto> Entries,
    IReadOnlyList<AffectedAppointmentDto> AffectedAppointments);

public sealed record OwnerAvailabilityEntryDto(
    Guid Id,
    Guid CollaboratorId,
    string CollaboratorName,
    string BranchName,
    AvailabilityChangeStatus Status,
    CollaboratorTimeOffType? Type,
    bool AllDay,
    DateTime? LocalStart,
    DateTime? LocalEnd,
    bool IsScheduleRequest,
    bool UseBranchHours,
    IReadOnlyList<OwnerWorkPeriodDto> Periods,
    int AffectedAppointments,
    DateTimeOffset CreatedAtUtc);

public sealed record OwnerWorkPeriodDto(
    ScheduleDay DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime);

public sealed record AffectedAppointmentDto(
    Guid Id,
    Guid CollaboratorId,
    string BranchName,
    string CollaboratorName,
    string ClientPhoneNumber,
    DateTime LocalStart,
    DateTime LocalEnd);

public sealed class GetOwnerAvailabilityHandler(
    OwnerAvailabilityAccess access,
    IAvailabilityRequestRepository requests,
    ICollaboratorRepository collaborators,
    ICollaboratorAvailabilityRepository availability)
    : IRequestHandler<GetOwnerAvailabilityQuery, OwnerAvailabilityDto>
{
    public async Task<OwnerAvailabilityDto> Handle(
        GetOwnerAvailabilityQuery request,
        CancellationToken ct)
    {
        if (request.CollaboratorId == Guid.Empty || request.Year is < 2000 or > 2100 || request.Month is < 1 or > 12)
            throw new ArgumentException("El colaborador, el año y el mes son requeridos.");

        var branch = await access.ResolveBranchAsync(request.CollaboratorId, ct);
        var collaborator = await collaborators.GetByIdAsync(request.CollaboratorId, ct)
            ?? throw new ArgumentException("El colaborador no existe.");
        var monthStartDate = new DateOnly(request.Year, request.Month, 1);
        var monthEndDate = monthStartDate.AddMonths(1);
        var monthStartUtc = BranchTimeZone.ToUtc(monthStartDate, branch.TimeZoneId);
        var monthEndUtc = BranchTimeZone.ToUtc(monthEndDate, branch.TimeZoneId);
        var scheduleRequests = await requests.GetOwnerScheduleRequestsAsync(
            request.CollaboratorId, monthStartUtc, monthEndUtc, ct);
        var timeOffItems = await requests.GetOwnerTimeOffAsync(
            request.CollaboratorId, monthStartUtc, monthEndUtc, ct);
        var entries = new List<OwnerAvailabilityEntryDto>();
        var appointments = await availability.GetFutureAppointmentsAsync(
            request.CollaboratorId, ct);
        foreach (var item in scheduleRequests)
        {
            var proposedHours = item.ToWorkingHours();
            var impacted = item.Status == AvailabilityChangeStatus.Pending
                ? appointments.Count(appointment => !proposedHours.Covers(
                    BranchTimeZone.ToBranchTime(
                        appointment.StartDateTime,
                        branch.TimeZoneId).DateTime,
                    BranchTimeZone.ToBranchTime(
                        appointment.EndDateTime,
                        branch.TimeZoneId).DateTime))
                : 0;
            entries.Add(new OwnerAvailabilityEntryDto(
                item.Id,
                request.CollaboratorId,
                collaborator.FullName,
                branch.Name,
                item.Status,
                null,
                false,
                null,
                null,
                true,
                item.UseBranchHours,
                item.Periods.Select(period => new OwnerWorkPeriodDto(
                    period.DayOfWeek,
                    period.StartTime,
                    period.EndTime)).ToList(),
                impacted,
                item.CreatedAtUtc));
        }

        foreach (var item in timeOffItems)
        {
            var shouldCount = item.Status is
                AvailabilityChangeStatus.Pending or
                AvailabilityChangeStatus.Approved;
            entries.Add(new OwnerAvailabilityEntryDto(
                item.Id,
                request.CollaboratorId,
                collaborator.FullName,
                branch.Name,
                item.Status,
                item.Type,
                item.AllDay,
                BranchTimeZone.ToBranchTime(item.StartAtUtc, branch.TimeZoneId).DateTime,
                BranchTimeZone.ToBranchTime(item.EndAtUtc, branch.TimeZoneId).DateTime,
                false,
                false,
                [],
                shouldCount
                    ? appointments.Count(appointment => item.Overlaps(
                        appointment.StartDateTime,
                        appointment.EndDateTime))
                    : 0,
                item.CreatedAtUtc));
        }

        var affectedAppointments = appointments.Where(appointment =>
            timeOffItems.Any(item => item.Status == AvailabilityChangeStatus.Approved &&
                item.Overlaps(appointment.StartDateTime, appointment.EndDateTime)));
        return new OwnerAvailabilityDto(
            entries
                .OrderBy(entry =>
                    entry.Status != AvailabilityChangeStatus.Pending)
                .ThenByDescending(entry => entry.CreatedAtUtc)
                .ToList(),
            affectedAppointments.Select(appointment => new AffectedAppointmentDto(
                appointment.Id,
                request.CollaboratorId,
                branch.Name,
                collaborator.FullName,
                appointment.User.PhoneNumber,
                BranchTimeZone.ToBranchTime(
                    appointment.StartDateTime,
                    branch.TimeZoneId).DateTime,
                BranchTimeZone.ToBranchTime(
                    appointment.EndDateTime,
                    branch.TimeZoneId).DateTime)).ToList());
    }
}
