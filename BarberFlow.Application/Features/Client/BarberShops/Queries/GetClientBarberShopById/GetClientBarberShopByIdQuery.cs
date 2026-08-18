using MediatR;

namespace BarberFlow.Application.Features.Client.BarberShops.Queries.GetClientBarberShopById;

public sealed record GetClientBarberShopByIdQuery(Guid BarberShopId)
    : IRequest<ClientBarberShopDetailDto?>;
