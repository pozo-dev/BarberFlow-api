using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Barber.Availability.WorkingHours;

public sealed record RequestWorkingHoursCommand(Guid CollaboratorId, bool UseBranchHours, IReadOnlyList<WorkPeriodDto> Periods) : IRequest<Guid>;

public sealed class RequestWorkingHoursHandler(BarberAvailabilityAccess access, ICollaboratorAvailabilityRepository repository,
    IAvailabilityRequestRepository requests, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    : IRequestHandler<RequestWorkingHoursCommand, Guid>
{
    public async Task<Guid> Handle(RequestWorkingHoursCommand request, CancellationToken ct)
    {
        await access.ResolveAsync(request.CollaboratorId, ct);
        if (request.Periods is null || request.Periods.Any(x => x is null))
            throw new ArgumentException("Los tramos son requeridos.");
        var proposal = new CollaboratorWorkingHours(request.CollaboratorId, request.UseBranchHours,
            request.Periods.Select(x => new CollaboratorWorkPeriod(x.DayOfWeek, x.StartTime, x.EndTime)));
        await using var mutation = await repository.BeginMutationAsync([request.CollaboratorId], ct);
        var latest = await requests.GetLatestScheduleRequestAsync(request.CollaboratorId, ct);
        if (latest?.Status == AvailabilityChangeStatus.Pending)
            throw new AvailabilityConflictException("Ya tienes una solicitud de horario pendiente. Retírala antes de enviar otra.");
        var item = new CollaboratorScheduleRequest(proposal, currentUser.ProfileId);
        requests.Add(item);
        await unitOfWork.SaveChangesAsync(ct);
        await mutation.CommitAsync(ct);
        return item.Id;
    }
}
