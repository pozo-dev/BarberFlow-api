using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Common.Time;
using BarberFlow.Domain.Enums;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Owner.Appointments.Queries.GetOwnerAppointments;

public sealed record GetOwnerAppointmentsQuery(DateOnly Date, Guid? BranchId, Guid? CollaboratorId) : IRequest<IReadOnlyList<OwnerAppointmentDto>>;
public sealed class GetOwnerAppointmentsHandler : IRequestHandler<GetOwnerAppointmentsQuery, IReadOnlyList<OwnerAppointmentDto>>
{
    private readonly ICurrentUserService _currentUser; private readonly IUserProfileRepository _profiles; private readonly IOwnerAppointmentRepository _appointments;
    public GetOwnerAppointmentsHandler(ICurrentUserService currentUser, IUserProfileRepository profiles, IOwnerAppointmentRepository appointments) => (_currentUser, _profiles, _appointments) = (currentUser, profiles, appointments);
    public async Task<IReadOnlyList<OwnerAppointmentDto>> Handle(GetOwnerAppointmentsQuery request, CancellationToken ct)
    {
        if (_currentUser.ProfileId == Guid.Empty) throw new CurrentProfileUnavailableException();
        var profile = await _profiles.GetByIdAsync(_currentUser.ProfileId, ct);
        if (profile?.BarberShopId is null) throw new ForbiddenAccessException();
        var records = await _appointments.GetByBarberShopIdAsync(profile.BarberShopId.Value, ct);
        return records.Where(item => (!request.BranchId.HasValue || item.BranchId == request.BranchId) && (!request.CollaboratorId.HasValue || item.CollaboratorId == request.CollaboratorId)).Select(item => Map(item, request.Date)).Where(item => item is not null).Cast<OwnerAppointmentDto>().OrderBy(item => item.StartAtUtc).ToList();
    }
    private static OwnerAppointmentDto? Map(OwnerAppointmentRecord item, DateOnly date)
    {
        var localStart = BranchTimeZone.ToBranchTime(item.StartAtUtc, item.TimeZoneId); if (DateOnly.FromDateTime(localStart.DateTime) != date) return null; var localEnd = BranchTimeZone.ToBranchTime(item.EndAtUtc, item.TimeZoneId);
        return new OwnerAppointmentDto { Id = item.Id, BranchId = item.BranchId, BranchName = item.BranchName, ClientName = item.ClientName, ClientPhoneNumber = item.ClientPhoneNumber, CollaboratorId = item.CollaboratorId, CollaboratorName = item.CollaboratorName, StartAtUtc = item.StartAtUtc, EndAtUtc = item.EndAtUtc, LocalStartTime = TimeOnly.FromDateTime(localStart.DateTime), LocalEndTime = TimeOnly.FromDateTime(localEnd.DateTime), Status = (AppointmentStatus)item.Status, Services = item.Services.Select(service => new OwnerAppointmentServiceDto { Name = service.Name, Price = service.Price }).ToList() };
    }
}
public sealed class OwnerAppointmentDto { public Guid Id { get; init; } public Guid BranchId { get; init; } public string BranchName { get; init; } = string.Empty; public string ClientName { get; init; } = string.Empty; public string ClientPhoneNumber { get; init; } = string.Empty; public Guid CollaboratorId { get; init; } public string CollaboratorName { get; init; } = string.Empty; public DateTimeOffset StartAtUtc { get; init; } public DateTimeOffset EndAtUtc { get; init; } public TimeOnly LocalStartTime { get; init; } public TimeOnly LocalEndTime { get; init; } public AppointmentStatus Status { get; init; } public IReadOnlyList<OwnerAppointmentServiceDto> Services { get; init; } = []; }
public sealed class OwnerAppointmentServiceDto { public string Name { get; init; } = string.Empty; public decimal Price { get; init; } }
