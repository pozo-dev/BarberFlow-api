using BarberFlow.Application.Common.Exceptions;
using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.BarberShops.Exceptions;
using BarberFlow.Domain.Interfaces;
using MediatR;

namespace BarberFlow.Application.Features.BarberShops.Commands.UpdateBarberShop;
public class UpdateBarberShopCommandHandler
    : IRequestHandler<UpdateBarberShopCommand, UpdateBarberShopResponseDto>
{
    private readonly IBarberShopRepository _barberShopRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBarberShopCommandHandler(
        IBarberShopRepository barberShopRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _barberShopRepository = barberShopRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateBarberShopResponseDto> Handle(
        UpdateBarberShopCommand request,
        CancellationToken cancellationToken)
    {
        ValidateImages(request.Logo, request.Banner);

        var barberShop =
            await _barberShopRepository.GetByOwnerUserIdAsync(
                _currentUserService.UserId,
                cancellationToken);

        if (barberShop is null)
        {
            throw new BarberShopNotFoundException();
        }

        barberShop.Update(
            request.Name,
            request.Description,
            request.Logo,
            request.Banner);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateBarberShopResponseDto
        {
            BarberShopId = barberShop.Id
        };
    }

    private static void ValidateImages(byte[] logo, byte[] banner)
    {
        if (logo.Length == 0)
        {
            throw new ValidationException("El logo es requerido.");
        }

        if (banner.Length == 0)
        {
            throw new ValidationException("El banner es requerido.");
        }
    }
}
