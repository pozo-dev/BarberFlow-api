using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.BarberShops.Exceptions;
using MediatR;

namespace BarberFlow.Application.Features.BarberShops.Queries.GetMyBarberShop
{
    public class GetMyBarberShopHandler
        : IRequestHandler<GetMyBarberShopQuery, GetMyBarberShopResponseDto>
    {
        private readonly IBarberShopRepository _barberShopRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyBarberShopHandler(
            IBarberShopRepository barberShopRepository,
            ICurrentUserService currentUserService)
        {
            _barberShopRepository = barberShopRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GetMyBarberShopResponseDto> Handle(
            GetMyBarberShopQuery request,
            CancellationToken cancellationToken)
        {
            var barberShop = await _barberShopRepository
                .GetByOwnerUserIdAsync(
                    _currentUserService.UserId,
                    cancellationToken);

            if (barberShop is null)
            {
                throw new BarberShopNotFoundException();
            }

            var mainBranch = barberShop.Branches
                .SingleOrDefault(branch => branch.IsMain);

            return new GetMyBarberShopResponseDto
            {
                Id = barberShop.Id,
                Name = barberShop.Name,
                Description = barberShop.Description,
                Logo = barberShop.Logo,
                Banner = barberShop.Banner,
                PhoneNumber = mainBranch?.PhoneNumber ?? string.Empty,
                Address = mainBranch?.Address ?? string.Empty,
                City = mainBranch?.City ?? string.Empty,
                //OpenTime = barberShop.OpenTime,
                //CloseTime = barberShop.CloseTime
            };
        }
    }
}
