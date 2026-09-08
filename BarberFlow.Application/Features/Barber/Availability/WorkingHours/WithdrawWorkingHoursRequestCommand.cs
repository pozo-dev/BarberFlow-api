using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Barber.Availability.WorkingHours;

public sealed record WithdrawWorkingHoursRequestCommand(Guid CollaboratorId, Guid RequestId) : IRequest;

public sealed class WithdrawWorkingHoursRequestHandler(BarberAvailabilityAccess access, ICollaboratorAvailabilityRepository repository,
    IAvailabilityRequestRepository requests, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    : IRequestHandler<WithdrawWorkingHoursRequestCommand>
{
    public async Task Handle(WithdrawWorkingHoursRequestCommand request, CancellationToken ct)
    {
        await access.ResolveAsync(request.CollaboratorId, ct);
        await using var mutation = await repository.BeginMutationAsync([request.CollaboratorId], ct);
        var item = await requests.GetScheduleRequestAsync(request.RequestId, ct);
        if (item is null || item.CollaboratorId != request.CollaboratorId) throw new ArgumentException("La solicitud no existe.");
        item.Decide(AvailabilityChangeStatus.Withdrawn, currentUser.ProfileId);
        await unitOfWork.SaveChangesAsync(ct);
        await mutation.CommitAsync(ct);
    }
}
