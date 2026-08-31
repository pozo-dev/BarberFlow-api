using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.BarberShops.Exceptions;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.BarberShops.Queries.GetMyBarberShop
{
    public class GetMyBarberShopHandler
        : IRequestHandler<GetMyBarberShopQuery, GetMyBarberShopResponseDto>
    {
        private readonly IBarberShopRepository _barberShopRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IServiceRepository _serviceRepository;
        private readonly ICollaboratorRepository _collaboratorRepository;

        public GetMyBarberShopHandler(
            IBarberShopRepository barberShopRepository,
            ICurrentUserService currentUserService,
            IServiceRepository serviceRepository,
            ICollaboratorRepository collaboratorRepository)
        {
            _barberShopRepository = barberShopRepository;
            _currentUserService = currentUserService;
            _serviceRepository = serviceRepository;
            _collaboratorRepository = collaboratorRepository;
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
            var services = await _serviceRepository.GetByBarberShopIdAsync(
                barberShop.Id,
                cancellationToken);
            var collaborators = await _collaboratorRepository.GetByBarberShopIdAsync(
                barberShop.Id,
                cancellationToken);
            var missingSetupItems = GetMissingSetupItems(
                barberShop.Branches,
                services,
                collaborators);

            return new GetMyBarberShopResponseDto
            {
                Id = barberShop.Id,
                Name = barberShop.Name,
                Description = barberShop.Description,
                Logo = barberShop.Logo,
                Banner = barberShop.Banner,
                PhoneNumber = mainBranch?.PhoneNumber ?? string.Empty,
                Address = mainBranch?.Address ?? string.Empty,
                LocationSearchId = mainBranch?.LocationSearchId,
                LocationDisplayName = mainBranch?.LocationSearch?.DisplayName ?? string.Empty,
                MissingSetupItems = missingSetupItems,
                //OpenTime = barberShop.OpenTime,
                //CloseTime = barberShop.CloseTime
            };
        }

        private static IReadOnlyList<string> GetMissingSetupItems(
            IEnumerable<Branch> branches,
            IEnumerable<Service> services,
            IEnumerable<Collaborator> collaborators)
        {
            var activeBranches = branches.Where(branch => branch.IsActive).ToList();
            var hasCompleteSchedule = activeBranches.Any(branch =>
                branch.Schedules.Count == Branch.WeeklyScheduleDays &&
                branch.Schedules.Any(schedule => !schedule.IsClosed) &&
                branch.Schedules.Select(schedule => schedule.DayOfWeek).Distinct().Count() ==
                    Branch.WeeklyScheduleDays);
            var hasActiveService = services.Any(service => service.IsActive);
            var hasActiveCollaborator = collaborators.Any(collaborator =>
                collaborator.IsActive && collaborator.Branch.IsActive);

            var items = new List<string>();
            if (!hasCompleteSchedule) items.Add("schedules");
            if (!hasActiveService) items.Add("services");
            if (!hasActiveCollaborator) items.Add("collaborators");
            if (items.Count == 0 && !activeBranches.Any(branch =>
                    branch.Schedules.Count == Branch.WeeklyScheduleDays &&
                    branch.Schedules.Any(schedule => !schedule.IsClosed) &&
                    collaborators.Any(collaborator =>
                        collaborator.BranchId == branch.Id && collaborator.IsActive)))
            {
                items.Add("branchConfiguration");
            }
            return items;
        }
    }
}
