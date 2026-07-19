using BarberFlow.Application.Features.Services.Exceptions;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Services.Commands.DeleteService
{
    public class DeleteServiceHandler : IRequestHandler<DeleteServiceCommand>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteServiceHandler(
            IServiceRepository serviceRepository,
            IUnitOfWork unitOfWork)
        {
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            DeleteServiceCommand request,
            CancellationToken cancellationToken)
        {
            var service = await _serviceRepository
                .GetByIdAsync(request.Id, cancellationToken);

            if (service is null)
                throw new ServiceNotFoundException();

            _serviceRepository.Remove(service);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
