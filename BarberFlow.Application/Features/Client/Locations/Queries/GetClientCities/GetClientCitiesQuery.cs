using MediatR;

namespace BarberFlow.Application.Features.Client.Locations.Queries.GetClientCities;

public sealed record GetClientCitiesQuery : IRequest<IReadOnlyList<string>>;
