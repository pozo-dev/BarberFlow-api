using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Branches.Queries.GetMainBranch
{
    public class GetMainBranchQueryHandler
        : IRequestHandler<GetMainBranchQuery, GetMainBranchResponseDto>
    {
        private readonly IBarberShopRepository _barberShopRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMainBranchQueryHandler(
            IBarberShopRepository barberShopRepository,
            IBranchRepository branchRepository,
            ICurrentUserService currentUserService)
        {
            _barberShopRepository = barberShopRepository;
            _branchRepository = branchRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GetMainBranchResponseDto> Handle(
            GetMainBranchQuery request,
            CancellationToken cancellationToken)
        {
            var barberShop =
                await _barberShopRepository.GetByOwnerUserIdAsync(
                    _currentUserService.UserId,
                    cancellationToken);

            if (barberShop is null)
                throw new InvalidOperationException(
                    "La barbería no existe.");

            var branch =
                await _branchRepository.GetMainBranchAsync(
                    barberShop.Id,
                    cancellationToken);

            if (branch is null)
                throw new InvalidOperationException(
                    "No existe sucursal principal.");

            var monday =
                branch.Schedules
                    .FirstOrDefault(x =>
                        x.DayOfWeek == BarberFlow.Domain.Enums.ScheduleDay.Monday);

            return new GetMainBranchResponseDto
            {
                BranchId = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                LocationSearchId = branch.LocationSearchId,
                LocationDisplayName = branch.LocationSearch.DisplayName,
                PhoneNumber = branch.PhoneNumber,
                OpenTime = monday?.OpenTime.ToTimeSpan() ?? TimeSpan.Zero,
                CloseTime = monday?.CloseTime.ToTimeSpan() ?? TimeSpan.Zero,
                IsActive = branch.IsActive
            };
        }
    }
}
