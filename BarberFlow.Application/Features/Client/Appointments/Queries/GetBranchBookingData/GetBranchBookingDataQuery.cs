using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Queries.GetBranchBookingData;

public sealed record GetBranchBookingDataQuery(Guid BranchId) : IRequest<ClientBranchBookingDataDto?>;

public sealed class GetBranchBookingDataHandler : IRequestHandler<GetBranchBookingDataQuery, ClientBranchBookingDataDto?>
{
    private readonly IBranchRepository _branches;
    private readonly IServiceRepository _services;

    public GetBranchBookingDataHandler(IBranchRepository branches, IServiceRepository services)
    {
        _branches = branches;
        _services = services;
    }

    public async Task<ClientBranchBookingDataDto?> Handle(GetBranchBookingDataQuery request, CancellationToken cancellationToken)
    {
        var branch = await _branches.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null || !branch.IsActive || !branch.BarberShop.IsActive) return null;
        var services = await _services.GetByBarberShopIdAsync(branch.BarberShopId, cancellationToken);
        return new ClientBranchBookingDataDto
        {
            Id = branch.Id,
            Name = branch.Name,
            Address = branch.Address,
            LocationDisplayName = branch.LocationSearch.DisplayName,
            Services = services.Where(x => x.IsActive).OrderBy(x => x.DisplayOrder).Select(x => new ClientBookingServiceDto
            {
                Id = x.Id, Name = x.Name, Description = x.Description, Price = x.Price, DurationMinutes = (int)x.Duration.TotalMinutes
            }).ToList()
        };
    }
}

public sealed class ClientBranchBookingDataDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string LocationDisplayName { get; init; } = string.Empty;
    public IReadOnlyList<ClientBookingServiceDto> Services { get; init; } = Array.Empty<ClientBookingServiceDto>();
}

public sealed class ClientBookingServiceDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int DurationMinutes { get; init; }
}
