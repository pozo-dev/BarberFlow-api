using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Client.BarberShops.Queries.GetClientBarberShopById;

public sealed class GetClientBarberShopByIdHandler
    : IRequestHandler<GetClientBarberShopByIdQuery, ClientBarberShopDetailDto?>
{
    private readonly IBarberShopRepository _barberShopRepository;

    public GetClientBarberShopByIdHandler(IBarberShopRepository barberShopRepository)
    {
        _barberShopRepository = barberShopRepository;
    }

    public async Task<ClientBarberShopDetailDto?> Handle(
        GetClientBarberShopByIdQuery request,
        CancellationToken cancellationToken)
    {
        var shop = await _barberShopRepository.GetActiveWithDetailsAsync(
            request.BarberShopId,
            cancellationToken);
        if (shop is null || shop.Branches.Count == 0)
            return null;

        return new ClientBarberShopDetailDto
        {
            Id = shop.Id,
            Name = shop.Name,
            Description = shop.Description,
            Logo = shop.Logo.Length == 0 ? null : Convert.ToBase64String(shop.Logo),
            Banner = shop.Banner.Length == 0 ? null : Convert.ToBase64String(shop.Banner),
            Branches = shop.Branches
                .OrderByDescending(branch => branch.IsMain)
                .ThenBy(branch => branch.Name)
                .Select(branch => new ClientBranchDto
                {
                    Id = branch.Id,
                    Name = branch.Name,
                    Address = branch.Address,
                    LocationSearchId = branch.LocationSearchId,
                    LocationDisplayName = branch.LocationSearch.DisplayName,
                    PhoneNumber = branch.PhoneNumber,
                    IsMain = branch.IsMain,
                    Schedules = branch.Schedules
                        .OrderBy(schedule => schedule.DayOfWeek)
                        .Select(schedule => new ClientScheduleDto
                        {
                            DayOfWeek = (int)schedule.DayOfWeek,
                            OpenTime = schedule.OpenTime,
                            CloseTime = schedule.CloseTime,
                            IsClosed = schedule.IsClosed
                        })
                        .ToList()
                })
                .ToList()
        };
    }
}
