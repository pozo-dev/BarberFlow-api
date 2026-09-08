using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Owner.Availability;

public sealed record GetOwnerAvailabilityQuery : IRequest<OwnerAvailabilityDto>;

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
        var shopId = await access.GetShopIdAsync(ct);
        var collaboratorsById = (await collaborators.GetByBarberShopIdAsync(
                shopId, ct))
            .ToDictionary(item => item.Id);
        var scheduleRequests = await requests.GetOwnerScheduleRequestsAsync(
            shopId, ct);
        var timeOffItems = await requests.GetOwnerTimeOffAsync(shopId, ct);
        var entries = new List<OwnerAvailabilityEntryDto>();

        foreach (var collaboratorId in scheduleRequests
                     .Select(item => item.CollaboratorId)
                     .Concat(timeOffItems.Select(item => item.CollaboratorId))
                     .Distinct())
        {
            if (!collaboratorsById.TryGetValue(
                    collaboratorId, out var collaborator))
            {
                continue;
            }

            var appointments = await availability.GetFutureAppointmentsAsync(
                collaboratorId, ct);
            foreach (var item in scheduleRequests.Where(
                         entry => entry.CollaboratorId == collaboratorId))
            {
                var proposedHours = item.ToWorkingHours();
                var impacted = item.Status == AvailabilityChangeStatus.Pending
                    ? appointments.Count(appointment => !proposedHours.Covers(
                        BranchTimeZone.ToBranchTime(
                            appointment.StartDateTime,
                            collaborator.Branch.TimeZoneId).DateTime,
                        BranchTimeZone.ToBranchTime(
                            appointment.EndDateTime,
                            collaborator.Branch.TimeZoneId).DateTime))
                    : 0;
                entries.Add(new OwnerAvailabilityEntryDto(
                    item.Id,
                    collaboratorId,
                    collaborator.FullName,
                    collaborator.Branch.Name,
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

            foreach (var item in timeOffItems.Where(
                         entry => entry.CollaboratorId == collaboratorId))
            {
                var shouldCount = item.Status is
                    AvailabilityChangeStatus.Pending or
                    AvailabilityChangeStatus.Approved;
                entries.Add(new OwnerAvailabilityEntryDto(
                    item.Id,
                    collaboratorId,
                    collaborator.FullName,
                    collaborator.Branch.Name,
                    item.Status,
                    item.Type,
                    item.AllDay,
                    BranchTimeZone.ToBranchTime(
                        item.StartAtUtc,
                        collaborator.Branch.TimeZoneId).DateTime,
                    BranchTimeZone.ToBranchTime(
                        item.EndAtUtc,
                        collaborator.Branch.TimeZoneId).DateTime,
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
        }

        var affectedAppointments = await requests.GetAffectedAppointmentsAsync(
            shopId, ct);
        return new OwnerAvailabilityDto(
            entries
                .OrderBy(entry =>
                    entry.Status != AvailabilityChangeStatus.Pending)
                .ThenByDescending(entry => entry.CreatedAtUtc)
                .ToList(),
            affectedAppointments.Select(appointment => new AffectedAppointmentDto(
                appointment.Id,
                appointment.Branch.Name,
                appointment.Collaborator.FullName,
                appointment.User.PhoneNumber,
                BranchTimeZone.ToBranchTime(
                    appointment.StartDateTime,
                    appointment.Branch.TimeZoneId).DateTime,
                BranchTimeZone.ToBranchTime(
                    appointment.EndDateTime,
                    appointment.Branch.TimeZoneId).DateTime)).ToList());
    }
}
