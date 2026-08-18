using MediatR;

namespace BarberFlow.Application.Features.Client.Locations.Queries.GetClientCities;

public sealed class GetClientCitiesHandler
    : IRequestHandler<GetClientCitiesQuery, IReadOnlyList<string>>
{
    private readonly IBarberShopRepository _barberShopRepository;

    public GetClientCitiesHandler(IBarberShopRepository barberShopRepository)
    {
        _barberShopRepository = barberShopRepository;
    }

    public Task<IReadOnlyList<string>> Handle(
        GetClientCitiesQuery request,
        CancellationToken cancellationToken)
    {
        return _barberShopRepository.GetActiveCitiesAsync(cancellationToken);
    }
}
