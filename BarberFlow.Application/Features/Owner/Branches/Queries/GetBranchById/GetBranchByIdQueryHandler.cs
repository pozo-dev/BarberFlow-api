using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Branches.Queries.GetBranchById
{
    public class GetBranchByIdQueryHandler
    : IRequestHandler<GetBranchByIdQuery, BranchDetailResponseDto>
    {
        private readonly IBranchRepository _branchRepository;

        private readonly ICurrentUserService _currentUserService;

        public GetBranchByIdQueryHandler(
            IBranchRepository branchRepository,
            ICurrentUserService currentUserService)
        {
            _branchRepository = branchRepository;
            _currentUserService = currentUserService;
        }

        public async Task<BranchDetailResponseDto> Handle(
            GetBranchByIdQuery request,
            CancellationToken cancellationToken)
        {
            var ownerId = _currentUserService.UserId;

            var branch =
                await _branchRepository.GetByIdAsync(
                    request.Id,
                    cancellationToken);

            if (branch is null)
                throw new ValidationException(
                    "Sucursal no encontrada.");

            if (branch.BarberShop.OwnerUserId != ownerId)
                throw new ValidationException(
                    "No tienes acceso a esta sucursal.");

            return new BranchDetailResponseDto
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address,
                LocationSearchId = branch.LocationSearchId,
                LocationDisplayName = branch.LocationSearch.DisplayName,
                PhoneNumber = branch.PhoneNumber,
                IsMain = branch.IsMain,
                IsActive = branch.IsActive
            };
        }
    }
}
