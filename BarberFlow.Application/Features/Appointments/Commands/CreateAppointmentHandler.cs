using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Common.Time;
using BarberFlow.Application.Features.Appointments.DTOs;
using BarberFlow.Application.Features.Appointments.Exceptions;
using BarberFlow.Application.Features.Services.Exceptions;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Appointments.Commands
{
    public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
    {
        private readonly IAppointmentRepository _appointments;
        private readonly IUserRepository _users;
        private readonly IBranchRepository _branches;
        private readonly ICollaboratorRepository _collaborators;
        private readonly IServiceRepository _services;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CreateAppointmentHandler(IAppointmentRepository appointments, IUserRepository users, IBranchRepository branches, ICollaboratorRepository collaborators, IServiceRepository services, IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _appointments = appointments;
            _users = users;
            _branches = branches;
            _collaborators = collaborators;
            _services = services;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var input = request.Appointment ?? throw new ArgumentException("Los datos de la cita son requeridos.");
            var user = await _users.GetByIdAsync(_currentUser.UserId, cancellationToken) ?? throw new UserNotFoundException();
            if (!user.IsActive) throw new UserNotActiveException();

            var branch = await _branches.GetByIdAsync(input.BranchId, cancellationToken);
            if (branch is null || !branch.IsActive || !branch.BarberShop.IsActive)
                throw new InvalidOperationException("La sucursal no est� disponible.");

            var serviceIds = input.ServiceIds.Distinct().ToList();
            if (serviceIds.Count == 0) throw new AtLeastOneServiceRequiredException();
            var requestedServiceIds = serviceIds.ToHashSet();
            var services = (await _services.GetByBarberShopIdAsync(branch.BarberShopId, cancellationToken))
                .Where(x => requestedServiceIds.Contains(x.Id))
                .ToList();
            if (services.Count != serviceIds.Count || services.Any(x => !x.IsActive))
                throw new ServiceNotFoundException();

            if (input.StartDateTime.Offset != TimeSpan.Zero)
                throw new ArgumentException("La fecha de la cita debe enviarse en UTC.");

            var localStart = BranchTimeZone.ToBranchTime(input.StartDateTime, branch.TimeZoneId);
            var duration = TimeSpan.FromTicks(services.Sum(x => x.Duration.Ticks));
            var end = input.StartDateTime.Add(duration);
            var localEnd = BranchTimeZone.ToBranchTime(end, branch.TimeZoneId);
            var scheduleDayOfWeek = (DayOfWeek)(((int)localStart.DayOfWeek + 6) % 7);
            var schedule = branch.Schedules.SingleOrDefault(x => x.DayOfWeek == scheduleDayOfWeek);
            if (schedule is null || schedule.IsClosed || localStart.TimeOfDay < schedule.OpenTime.ToTimeSpan() || localEnd.Date != localStart.Date || localEnd.TimeOfDay > schedule.CloseTime.ToTimeSpan())
                throw new InvalidOperationException("La hora seleccionada no est� disponible.");

            var candidates = await _collaborators.GetActiveByBranchIdAsync(branch.Id, cancellationToken);
            var professionalIds = input.ProfessionalId.HasValue
                ? candidates.Where(x => x.Id == input.ProfessionalId.Value).Select(x => x.Id)
                : candidates.Select(x => x.Id);
            var professionalId = Guid.Empty;
            foreach (var candidateId in professionalIds)
            {
                if (!await _appointments.ExistsOverlappingAppointmentAsync(branch.Id, candidateId, input.StartDateTime, end, cancellationToken))
                {
                    professionalId = candidateId;
                    break;
                }
            }
            if (professionalId == Guid.Empty) throw new AppointmentConflictException();

            var appointment = new Appointment(user.Id, branch.Id, professionalId, input.StartDateTime, end);
            appointment.AddServices(services, services.ToDictionary(x => x.Id, x => x.Price));
            _appointments.Add(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AppointmentDto { Id = appointment.Id, BranchId = branch.Id, ProfessionalId = professionalId, UserId = user.Id, StartDateTime = appointment.StartDateTime, EndDateTime = appointment.EndDateTime, Status = appointment.Status, ServiceIds = serviceIds, CreatedAt = appointment.CreatedAt };
        }
    }
}
