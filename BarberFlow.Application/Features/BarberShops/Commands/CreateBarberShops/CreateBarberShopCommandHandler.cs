using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.BarberShops.Commands.CreateBarberShop;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using MediatR;

namespace BarberFlow.Application.Features.BarberShops.Commands.CreateBarberShop
{
    public class CreateBarberShopCommandHandler
        : IRequestHandler<CreateBarberShopCommand, CreateBarberShopResponseDto>
    {
        private readonly IBarberShopRepository _barberShopRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBarberShopCommandHandler(
            IBarberShopRepository barberShopRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _barberShopRepository = barberShopRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateBarberShopResponseDto> Handle(
            CreateBarberShopCommand request,
            CancellationToken cancellationToken)
        {
            var ownerUserId = _currentUserService.UserId;

            var barberShop = await _barberShopRepository.GetByOwnerUserIdAsync(
                ownerUserId,
                cancellationToken);

            if (barberShop is not null)
            {
                throw new InvalidOperationException(
                    "El usuario ya posee una barbería.");
            }

            barberShop = new BarberShop(
                ownerUserId,
                request.Name,
                request.Description,
                request.PhoneNumber,
                request.Address,
                request.City,
                TimeOnly.FromTimeSpan(request.OpenTime),
                TimeOnly.FromTimeSpan(request.CloseTime));

            _barberShopRepository.Add(barberShop);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateBarberShopResponseDto
            {
                BarberShopId = barberShop.Id
            };
        }
    }
}