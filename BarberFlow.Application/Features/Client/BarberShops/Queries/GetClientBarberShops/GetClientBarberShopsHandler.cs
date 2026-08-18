using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Client.BarberShops.Queries.GetClientBarberShops;

public sealed class GetClientBarberShopsHandler
    : IRequestHandler<GetClientBarberShopsQuery, ClientBarberShopPageDto>
{
    private readonly IBarberShopRepository _barberShopRepository;

    public GetClientBarberShopsHandler(IBarberShopRepository barberShopRepository)
    {
        _barberShopRepository = barberShopRepository;
    }

    public async Task<ClientBarberShopPageDto> Handle(
        GetClientBarberShopsQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 20);
        var totalCount = await _barberShopRepository.CountActiveAsync(
            request.Search,
            request.City,
            cancellationToken);
        var shops = await _barberShopRepository.SearchActiveAsync(
            request.Search,
            request.City,
            (page - 1) * pageSize,
            pageSize,
            cancellationToken);

        var items = shops.Select(shop =>
        {
            var branches = shop.Branches
                .OrderByDescending(branch => branch.IsMain)
                .ThenBy(branch => branch.Name)
                .ToList();
            var branch = branches.First();

            return new ClientBarberShopListItemDto
            {
                Id = shop.Id,
                Name = shop.Name,
                Description = shop.Description,
                Logo = shop.Logo.Length == 0 ? null : Convert.ToBase64String(shop.Logo),
                LocationDisplayName = branch.LocationSearch.DisplayName,
                BranchName = branch.Name,
                BranchCount = branches.Count
            };
        }).ToList();

        return new ClientBarberShopPageDto
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasNextPage = page * pageSize < totalCount
        };
    }
}
