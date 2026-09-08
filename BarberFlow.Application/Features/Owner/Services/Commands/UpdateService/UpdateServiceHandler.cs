using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.BarberShops.Exceptions;
using BarberFlow.Application.Features.Services.Exceptions;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Services.Commands.UpdateService;
public class UpdateServiceHandler : IRequestHandler<UpdateServiceCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBarberShopRepository _barberShopRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateServiceHandler(
        ICurrentUserService currentUserService,
        IBarberShopRepository barberShopRepository,
        IServiceRepository serviceRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _barberShopRepository = barberShopRepository;
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpdateServiceCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var barberShop = await _barberShopRepository
            .GetByOwnerUserIdAsync(userId, cancellationToken);

        if (barberShop is null)
            throw new BarberShopNotFoundException();

        var service = await _serviceRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (service is null || service.BarberShopId != barberShop.Id)
            throw new ServiceNotFoundException();

        service.Update(
            request.Name,
            request.Price,
            TimeSpan.FromMinutes(request.DurationMinutes),
            request.Description,
            request.DisplayOrder);

        _serviceRepository.Update(service);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
