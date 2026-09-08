using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Owner.Availability;

public sealed record ReviewScheduleRequestCommand(
    Guid CollaboratorId,
    Guid RequestId,
    bool Approve) : IRequest;

public sealed class ReviewScheduleRequestHandler(
    OwnerAvailabilityAccess access,
    IAvailabilityRequestRepository requests,
    ICollaboratorAvailabilityRepository availability,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ReviewScheduleRequestCommand>
{
    public async Task Handle(ReviewScheduleRequestCommand request, CancellationToken ct)
    {
        var branch = await access.ResolveBranchAsync(request.CollaboratorId, ct);
        await using var mutation =
            await availability.BeginMutationAsync([request.CollaboratorId], ct);

        var item = await requests.GetScheduleRequestAsync(request.RequestId, ct);
        if (item is null || item.CollaboratorId != request.CollaboratorId)
        {
            throw new ArgumentException("La solicitud no existe.");
        }

        if (request.Approve)
        {
            var proposedHours = item.ToWorkingHours();
            var conflicts = (await availability.GetFutureAppointmentsAsync(
                    request.CollaboratorId, ct))
                .Count(appointment => !proposedHours.Covers(
                    BranchTimeZone.ToBranchTime(
                        appointment.StartDateTime,
                        branch.TimeZoneId).DateTime,
                    BranchTimeZone.ToBranchTime(
                        appointment.EndDateTime,
                        branch.TimeZoneId).DateTime));
            if (conflicts > 0)
            {
                throw new AvailabilityConflictException(
                    $"El cambio afecta {conflicts} cita(s). Gestiona esas reservas antes de aprobarlo.");
            }

            var current = await availability.GetWorkingHoursAsync(
                request.CollaboratorId, ct);
            if (current is null)
            {
                availability.Add(proposedHours);
            }
            else
            {
                availability.ReplaceWorkingHours(
                    current,
                    proposedHours.UseBranchHours,
                    proposedHours.Periods);
            }
        }

        item.Decide(
            request.Approve
                ? AvailabilityChangeStatus.Approved
                : AvailabilityChangeStatus.Rejected,
            currentUser.ProfileId);
        await unitOfWork.SaveChangesAsync(ct);
        await mutation.CommitAsync(ct);
    }
}
