using BarberFlow.Application.Features.Services.Exceptions;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Services.Commands.ToggleServiceStatus
{
    public class ToggleServiceStatusHandler : IRequestHandler<ToggleServiceStatusCommand>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ToggleServiceStatusHandler(
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            ToggleServiceStatusCommand request,
            CancellationToken cancellationToken)
        {
            var service = await _serviceRepository
                .GetByIdAsync(request.Id, cancellationToken);

            if (service is null)
                throw new ServiceNotFoundException();

            if (service.IsActive)
                service.Deactivate();
            else
                service.Activate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
