using BarberFlow.Application.Features.Services.Exceptions;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Services.Commands.DeleteService
{
    public class DeleteServiceHandler : IRequestHandler<DeleteServiceCommand>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly BarberFlow.Application.Common.Interfaces.ICurrentUserService _currentUserService;
        private readonly IBarberShopRepository _barberShopRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteServiceHandler(
            IServiceRepository serviceRepository,
            BarberFlow.Application.Common.Interfaces.ICurrentUserService currentUserService,
            IBarberShopRepository barberShopRepository,
            IUnitOfWork unitOfWork)
        {
            _serviceRepository = serviceRepository;
            _currentUserService = currentUserService;
            _barberShopRepository = barberShopRepository;
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

            var barberShop = await _barberShopRepository.GetByOwnerUserIdAsync(
                _currentUserService.UserId,
                cancellationToken);

            if (barberShop is null || service.BarberShopId != barberShop.Id)
                throw new ServiceNotFoundException();

            _serviceRepository.Remove(service);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
