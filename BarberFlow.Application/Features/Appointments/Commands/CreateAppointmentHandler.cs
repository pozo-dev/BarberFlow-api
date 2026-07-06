using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Appointments.DTOs;
using BarberFlow.Application.Features.Appointments.Exceptions;
using BarberFlow.Application.Features.BarberShop.Exceptions;
using BarberFlow.Application.Features.ServicePrice.Exceptions;
using BarberFlow.Application.Features.Services.Exceptions;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Appointments.Commands
{
    public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBarberShopRepository _barberShopRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServicePriceRepository _servicePriceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public CreateAppointmentHandler(
            IAppointmentRepository appointmentRepository,
            IUserRepository userRepository,
            IBarberShopRepository barberShopRepository,
            IServiceRepository serviceRepository,
            IServicePriceRepository servicePriceRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser)
        {
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _barberShopRepository = barberShopRepository;
            _serviceRepository = serviceRepository;
            _servicePriceRepository = servicePriceRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            // Validar usuario y barbería
            var userId = _currentUser.UserId;
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new UserNotFoundException();
            if (!user.IsActive)
                throw new UserNotActiveException();

            var barberShop = await _barberShopRepository.GetByIdAsync(request.Appointment.BarberShopId, cancellationToken);
            if (barberShop == null)
                throw new BarberShopNotFoundException();
            if (!barberShop.IsActive)
                throw new BarberShopNotActiveException();

            // Validar solapamiento de citas
            var overlapping = await _appointmentRepository.ExistsOverlappingAppointmentAsync(
                request.Appointment.BarberShopId,
                request.Appointment.StartDateTime,
                request.Appointment.EndDateTime,
                cancellationToken);

            if (overlapping)
                throw new AppointmentConflictException();

            // Asociar servicios
            var serviceIds = request.Appointment.ServiceIds;

            if (serviceIds == null || !serviceIds.Any())
                throw new AtLeastOneServiceRequiredException();

            var distinctServiceIds = serviceIds.Distinct().ToList();
            var services = await _serviceRepository.GetByIdsAsync(distinctServiceIds, cancellationToken);
            if (services.Count != distinctServiceIds.Count)
                throw new ServiceNotFoundException();
            if (services.Any(s => !s.IsActive))
                throw new ServiceNotActiveException();
            var prices = await _servicePriceRepository.GetCurrentPricesAsync(distinctServiceIds, cancellationToken);
            if (prices.Count != distinctServiceIds.Count)
                throw new ServicePriceNotFoundException();

            // Crear la cita
            var appointment = new Appointment(
                userId,
                request.Appointment.BarberShopId,
                request.Appointment.StartDateTime,
                request.Appointment.EndDateTime);

            appointment.AddServices(services, prices);
            _appointmentRepository.Add(appointment);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Mapear a DTO
            return new AppointmentDto
            {
                Id = appointment.Id,
                BarberShopId = appointment.BarberShopId,
                UserId = appointment.UserId,
                StartDateTime = appointment.StartDateTime,
                EndDateTime = appointment.EndDateTime,
                Status = appointment.Status,
                ServiceIds = request.Appointment.ServiceIds,
                CreatedAt = appointment.CreatedAt
            };
        }
    }
}
