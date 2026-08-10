using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.BarberShops.Commands.CreateBarberShop
{
    public class CreateBarberShopCommandHandler
        : IRequestHandler<CreateBarberShopCommand, CreateBarberShopResponseDto>
    {
        private readonly IBarberShopRepository _barberShopRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBarberShopCommandHandler(
            IBarberShopRepository barberShopRepository,
            IBranchRepository branchRepository,
            IUserProfileRepository userProfileRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _barberShopRepository = barberShopRepository;
            _branchRepository = branchRepository;
            _userProfileRepository = userProfileRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateBarberShopResponseDto> Handle(
            CreateBarberShopCommand request,
            CancellationToken cancellationToken)
        {
            ValidateImages(request.Logo, request.Banner);

            var ownerUserId = _currentUserService.UserId;
            var profileId = _currentUserService.ProfileId;

            if (profileId == Guid.Empty)
                throw new CurrentProfileUnavailableException();

            // Verificar que el usuario no tenga una barbería
            var existingBarberShop =
                await _barberShopRepository.GetByOwnerUserIdAsync(
                    ownerUserId,
                    cancellationToken);

            if (existingBarberShop is not null)
            {
                throw new ValidationException(
                    "El usuario ya posee una barbería.");
            }

            // Obtener el perfil del barbero
            var profile = await _userProfileRepository.GetByIdAsync(
                profileId,
                cancellationToken);

            if (profile is null)
                throw new UserProfileNotFoundException();

            // 1. Crear la barbería (marca)
            var barberShop = BarberShop.Create(
                ownerUserId,
                request.Name,
                request.Description,
                request.Logo,
                request.Banner);

            _barberShopRepository.Add(barberShop);

            // 2. Crear automáticamente la sucursal principal
            var mainBranch = Branch.Create(
                barberShop.Id,
                "Sucursal principal",
                request.Address,
                request.City,
                request.PhoneNumber,
                true);

            await _branchRepository.AddAsync(
                mainBranch,
                cancellationToken);

            // 3. Asociar el perfil a la barbería
            profile.AssignBarberShop(barberShop.Id);

            // 4. Guardar todo en una sola transacción
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateBarberShopResponseDto
            {
                BarberShopId = barberShop.Id
            };
        }

        private static void ValidateImages(byte[] logo, byte[] banner)
        {
            if (logo.Length == 0)
                throw new ValidationException("El logo es requerido.");

            if (banner.Length == 0)
                throw new ValidationException("El banner es requerido.");
        }
    }

}
