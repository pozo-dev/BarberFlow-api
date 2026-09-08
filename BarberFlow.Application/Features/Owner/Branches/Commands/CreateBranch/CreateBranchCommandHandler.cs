using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Branches.Exceptions;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Branches.Commands.CreateBranch;
public class CreateBranchCommandHandler
    : IRequestHandler<CreateBranchCommand, Guid>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILocationSearchRepository _locationSearchRepository;

    public CreateBranchCommandHandler(
        ICurrentUserService currentUserService,
        IUserProfileRepository userProfileRepository,
        IBranchRepository branchRepository,
        IUnitOfWork unitOfWork,
        ILocationSearchRepository locationSearchRepository)
    {
        _currentUserService = currentUserService;
        _userProfileRepository = userProfileRepository;
        _branchRepository = branchRepository;
        _unitOfWork = unitOfWork;
        _locationSearchRepository = locationSearchRepository;
    }

    public async Task<Guid> Handle(
        CreateBranchCommand request,
        CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        if (profileId == Guid.Empty)
            throw new CurrentProfileUnavailableException();

        var profile = await _userProfileRepository.GetByIdAsync(
            profileId,
            cancellationToken);

        if (profile == null)
            throw new UserProfileNotFoundException();

        if (profile.BarberShopId == null)
            throw new BarberShopAssociationException();

        var exists = await _branchRepository.ExistsByNameAsync(
            profile.BarberShopId.Value,
            request.Name.Trim(),
            cancellationToken);

        if (exists)
            throw new DuplicateBranchNameException(request.Name.Trim());

        if (!await _locationSearchRepository.ExistsAsync(request.LocationSearchId, cancellationToken))
            throw new ValidationException("La ubicación seleccionada no es válida.");

        var branch = Branch.Create(
            profile.BarberShopId.Value,
            request.Name,
            request.Address,
            request.LocationSearchId,
            request.PhoneNumber);

        await _branchRepository.AddAsync(
            branch,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return branch.Id;
    }
}
