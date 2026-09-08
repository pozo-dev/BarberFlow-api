using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Enums;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Barber.Availability.TimeOff;

public sealed record DeleteTimeOffCommand(Guid CollaboratorId, Guid TimeOffId) : IRequest;

public sealed class DeleteTimeOffHandler(BarberAvailabilityAccess access, ICollaboratorAvailabilityRepository repository, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<DeleteTimeOffCommand>
{
    public async Task Handle(DeleteTimeOffCommand request, CancellationToken ct)
    {
        await access.ResolveAsync(request.CollaboratorId, ct);
        await using var mutation = await repository.BeginMutationAsync([request.CollaboratorId], ct);
        var items = await repository.GetTimeOffAsync(request.CollaboratorId, ct);
        var item = items.SingleOrDefault(x => x.Id == request.TimeOffId);
        if (item is null) throw new ArgumentException("La ausencia no existe o ya finalizó.");
        item.Decide(AvailabilityChangeStatus.Withdrawn, currentUser.ProfileId);
        await unitOfWork.SaveChangesAsync(ct);
        await mutation.CommitAsync(ct);
    }
}
