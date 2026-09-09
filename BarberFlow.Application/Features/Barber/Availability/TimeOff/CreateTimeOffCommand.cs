using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Barber.Availability.TimeOff;

public sealed record CreateTimeOffCommand(Guid CollaboratorId, CollaboratorTimeOffType Type, bool AllDay,
    DateOnly StartDate, TimeOnly StartTime, DateOnly EndDate, TimeOnly EndTime) : IRequest<Guid>;

public sealed class CreateTimeOffHandler(BarberAvailabilityAccess access, ICollaboratorAvailabilityRepository repository, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<CreateTimeOffCommand, Guid>
{
    public async Task<Guid> Handle(CreateTimeOffCommand request, CancellationToken ct)
    {
        var (_, branch) = await access.ResolveAsync(request.CollaboratorId, ct);
        if (request.EndDate < request.StartDate || request.EndDate == DateOnly.MaxValue)
            throw new ArgumentException("La fecha final no puede ser anterior a la inicial.");
        if (request.Type == CollaboratorTimeOffType.Vacation && !request.AllDay)
            throw new ArgumentException("Las vacaciones se registran por días completos.");
        var start = BranchTimeZone.ToUtc(request.StartDate, request.AllDay ? TimeOnly.MinValue : request.StartTime, branch.TimeZoneId);
        // All-day dates are inclusive for people and half-open UTC intervals for overlap checks.
        var end = BranchTimeZone.ToUtc(request.AllDay ? request.EndDate.AddDays(1) : request.EndDate,
            request.AllDay ? TimeOnly.MinValue : request.EndTime, branch.TimeZoneId);
        if (end <= DateTimeOffset.UtcNow) throw new ArgumentException("La ausencia debe incluir tiempo futuro.");
        if (request.Type == CollaboratorTimeOffType.Break && (request.AllDay || request.StartDate != request.EndDate))
            throw new ArgumentException("El descanso debe indicar horas de un mismo día.");
        var item = new CollaboratorTimeOff(request.CollaboratorId, request.Type, start, end, request.AllDay, currentUser.ProfileId);
        await using var mutation = await repository.BeginMutationAsync([request.CollaboratorId], ct);
        var existing = await repository.GetTimeOffAsync(request.CollaboratorId, ct);
        if (existing.Any(x =>
                x.Status is AvailabilityChangeStatus.Pending or AvailabilityChangeStatus.Approved
                && x.Overlaps(start, end)))
            throw new AvailabilityConflictException("Ya tienes una ausencia o vacaciones en ese período.");
        var appointments = await repository.GetFutureAppointmentsAsync(request.CollaboratorId, ct);
        var conflicts = appointments.Count(x => item.Overlaps(x.StartDateTime, x.EndDateTime));
        if (request.Type == CollaboratorTimeOffType.Break && conflicts > 0)
            throw new AvailabilityConflictException($"Este descanso afecta {conflicts} cita(s). Elige otro horario o coordina su gestión con el Owner.");
        repository.Add(item);
        await unitOfWork.SaveChangesAsync(ct);
        await mutation.CommitAsync(ct);
        return item.Id;
    }
}
