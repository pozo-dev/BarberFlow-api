using BarberFlow.Application.Common.Interfaces;
using BarberFlow.Application.Features.BarberShops.Exceptions;
using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces;
using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Services.Commands.CreateService;
public class CreateServiceHandler : IRequestHandler<CreateServiceCommand, Guid>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBarberShopRepository _barberShopRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateServiceHandler(
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

    public async Task<Guid> Handle(
        CreateServiceCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var barberShop = await _barberShopRepository
            .GetByOwnerUserIdAsync(userId, cancellationToken);

        if (barberShop == null)
            throw new BarberShopNotFoundException();

        var service = Service.Create(
            barberShopId: barberShop.Id,
            name: request.Name,
            price: request.Price,
            duration: TimeSpan.FromMinutes(request.DurationMinutes),
            description: request.Description,
            displayOrder: request.DisplayOrder);

        await _serviceRepository.AddAsync(service, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return service.Id;
    }
}
