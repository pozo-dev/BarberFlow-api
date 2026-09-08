using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Appointments.DTOs;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Appointments.Queries;
public class GetUserAppointmentsHandler : IRequestHandler<GetUserAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICurrentUserService _currentUser;

    public GetUserAppointmentsHandler(IAppointmentRepository appointmentRepository, ICurrentUserService currentUser)
    {
        _appointmentRepository = appointmentRepository;
        _currentUser = currentUser;
    }

    public async Task<List<AppointmentDto>> Handle(GetUserAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var appointments = await _appointmentRepository.GetByUserIdAsync(userId, cancellationToken);
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
