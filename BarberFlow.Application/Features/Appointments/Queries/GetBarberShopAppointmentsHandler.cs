using BarberFlow.Application.Features.Appointments.DTOs;
using BarberFlow.Domain.Interfaces.Repositories;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Common.Exceptions;
using MediatR;

namespace BarberFlow.Application.Features.Appointments.Queries;
public class GetBarberShopAppointmentsHandler : IRequestHandler<GetBarberShopAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IBarberShopRepository _barberShopRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetBarberShopAppointmentsHandler(
        IAppointmentRepository appointmentRepository,
        IBarberShopRepository barberShopRepository,
        ICurrentUserService currentUserService)
    {
        _appointmentRepository = appointmentRepository;
        _barberShopRepository = barberShopRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<AppointmentDto>> Handle(GetBarberShopAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var barberShop = await _barberShopRepository.GetByIdAsync(
            request.BarberShopId,
            cancellationToken);

        if (barberShop is null || barberShop.OwnerUserId != _currentUserService.UserId)
            throw new ForbiddenAccessException();

        var appointments = await _appointmentRepository.GetByBarberShopIdAsync(request.BarberShopId, cancellationToken);
        return appointments.Select(a => new AppointmentDto
        {
            Id = a.Id,
            BranchId = a.BranchId,
            ProfessionalId = a.CollaboratorId,
            UserId = a.UserId,
            StartDateTime = a.StartDateTime,
            EndDateTime = a.EndDateTime,
            Status = a.Status,
            CreatedAt = a.CreatedAt
        }).ToList();
    }
}
