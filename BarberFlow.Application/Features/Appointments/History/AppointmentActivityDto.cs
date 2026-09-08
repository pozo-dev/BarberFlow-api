using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Enums;

namespace BarberFlow.Application.Features.Appointments.History;

public sealed class AppointmentActivityDto
{
    public Guid Id { get; init; }
    public AppointmentActivityType Type { get; init; }
    public Guid ActorProfileId { get; init; }
    public int ActorRoleId { get; init; }
    public DateTimeOffset OccurredAtUtc { get; init; }
    public DateTimeOffset? PreviousStartAtUtc { get; init; }
    public DateTimeOffset? NewStartAtUtc { get; init; }
    public Guid? RelatedAppointmentId { get; init; }
}

public static class AppointmentActivityMapper
{
    public static AppointmentActivityDto ToDto(AppointmentActivity activity) =>
        new()
        {
            Id = activity.Id,
            Type = activity.Type,
            ActorProfileId = activity.ActorProfileId,
            ActorRoleId = activity.ActorRoleId,
            OccurredAtUtc = activity.OccurredAtUtc,
            PreviousStartAtUtc = activity.PreviousStartAtUtc,
            NewStartAtUtc = activity.NewStartAtUtc,
            RelatedAppointmentId = activity.RelatedAppointmentId,
        };
}
