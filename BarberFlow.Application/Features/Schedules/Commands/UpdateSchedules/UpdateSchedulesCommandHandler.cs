using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.Branches.Exceptions;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Schedules.Commands.UpdateSchedules
{
    public class UpdateSchedulesCommandHandler
        : IRequestHandler<UpdateSchedulesCommand, UpdateSchedulesResponseDto>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSchedulesCommandHandler(
            ICurrentUserService currentUserService,
            IUserProfileRepository userProfileRepository,
            IBranchRepository branchRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _userProfileRepository = userProfileRepository;
            _branchRepository = branchRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateSchedulesResponseDto> Handle(
            UpdateSchedulesCommand request,
            CancellationToken cancellationToken)
        {
            var profileId = _currentUserService.ProfileId;

            if (profileId == Guid.Empty)
                throw new CurrentProfileUnavailableException();

            var profile = await _userProfileRepository.GetByIdAsync(
                profileId,
                cancellationToken);

            if (profile is null)
                throw new UserProfileNotFoundException();

            var branch = await _branchRepository.GetByIdAsync(
                request.BranchId,
                cancellationToken);

            if (branch is null)
                throw new BranchNotFoundException();

            if (branch.BarberShopId != profile.BarberShopId)
                throw new ForbiddenAccessException();

            try
            {
                branch.UpdateSchedules(request.Schedules
                    .Select(x => new BranchScheduleUpdate(
                        x.ScheduleId,
                        x.DayOfWeek,
                        TimeOnly.FromTimeSpan(x.OpenTime),
                        TimeOnly.FromTimeSpan(x.CloseTime),
                        x.IsClosed))
                    .ToList());
            }
            catch (Exception exception) when (
                exception is ArgumentException or InvalidOperationException)
            {
                throw new ValidationException(exception.Message);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateSchedulesResponseDto { BranchId = branch.Id };
        }
    }
}
