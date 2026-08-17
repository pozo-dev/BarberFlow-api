using MediatR;

namespace BarberFlow.Application.Features.Locations.Queries.SearchLocations;

public sealed record SearchLocationsQuery(string Search, int Take = 20) : IRequest<IReadOnlyList<LocationSearchDto>>;

public sealed record LocationSearchDto(int Id, string DisplayName);
