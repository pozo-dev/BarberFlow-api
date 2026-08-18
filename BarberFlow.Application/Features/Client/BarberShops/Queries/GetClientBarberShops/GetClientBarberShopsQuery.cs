using MediatR;

namespace BarberFlow.Application.Features.Client.BarberShops.Queries.GetClientBarberShops;

public sealed record GetClientBarberShopsQuery(
    string? Search,
    string? City,
    int Page,
    int PageSize) : IRequest<ClientBarberShopPageDto>;
