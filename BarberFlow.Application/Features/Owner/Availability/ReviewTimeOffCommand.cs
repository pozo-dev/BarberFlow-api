using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Owner.Availability;

public sealed record ReviewTimeOffCommand(
    Guid CollaboratorId,
    Guid TimeOffId,
    AvailabilityChangeStatus Decision) : IRequest;

public sealed class ReviewTimeOffHandler(
    OwnerAvailabilityAccess access,
    IAvailabilityRequestRepository requests,
    ICollaboratorAvailabilityRepository availability,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ReviewTimeOffCommand>
{
    public async Task Handle(ReviewTimeOffCommand request, CancellationToken ct)
    {
        await access.ResolveBranchAsync(request.CollaboratorId, ct);
        await using var mutation =
            await availability.BeginMutationAsync([request.CollaboratorId], ct);

        var item = await requests.GetTimeOffByIdAsync(request.TimeOffId, ct);
        if (item is null || item.CollaboratorId != request.CollaboratorId)
        {
            throw new ArgumentException("El registro no existe.");
        }

        if (request.Decision == AvailabilityChangeStatus.Approved)
        {
            if (item.EndAtUtc <= DateTimeOffset.UtcNow)
            {
                throw new ArgumentException("El período ya finalizó.");
            }

            var conflicts = (await availability.GetFutureAppointmentsAsync(
                    request.CollaboratorId, ct))
                .Count(appointment => item.Overlaps(
                    appointment.StartDateTime,
                    appointment.EndDateTime));
            if (conflicts > 0)
            {
                throw new AvailabilityConflictException(
                    $"Las vacaciones afectan {conflicts} cita(s). Gestiona esas reservas antes de aprobarlas.");
            }
        }

        item.Decide(request.Decision, currentUser.ProfileId);
        await unitOfWork.SaveChangesAsync(ct);
        await mutation.CommitAsync(ct);
    }
}
