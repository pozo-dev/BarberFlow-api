using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Services.Queries.GetMyServices
{
    public class GetMyServicesHandler
    : IRequestHandler<GetMyServicesQuery, List<ServiceDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IBarberShopRepository _barberShopRepository;
        private readonly IServiceRepository _serviceRepository;

        public GetMyServicesHandler(
            ICurrentUserService currentUserService,
            IBarberShopRepository barberShopRepository,
            IServiceRepository serviceRepository)
        {
            _currentUserService = currentUserService;
            _barberShopRepository = barberShopRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<List<ServiceDto>> Handle(
            GetMyServicesQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (userId == Guid.Empty)
                return new List<ServiceDto>();

            var barberShop = await _barberShopRepository
                .GetByOwnerUserIdAsync(userId, cancellationToken);

            if (barberShop is null)
                return new List<ServiceDto>();

            var services = await _serviceRepository
                .GetByBarberShopIdAsync(barberShop.Id, cancellationToken);

            return services.Select(x => new ServiceDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                DurationMinutes = (int)x.Duration.TotalMinutes,
                IsActive = x.IsActive,
                DisplayOrder = x.DisplayOrder
            }).ToList();
        }
    }
}
