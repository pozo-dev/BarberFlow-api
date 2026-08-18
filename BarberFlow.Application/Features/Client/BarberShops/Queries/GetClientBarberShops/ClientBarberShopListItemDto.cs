namespace BarberFlow.Application.Features.Client.BarberShops.Queries.GetClientBarberShops;

public sealed class ClientBarberShopListItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Logo { get; init; }
    public string LocationDisplayName { get; init; } = string.Empty;
    public string BranchName { get; init; } = string.Empty;
    public int BranchCount { get; init; }
}

public sealed class ClientBarberShopPageDto
{
    public IReadOnlyList<ClientBarberShopListItemDto> Items { get; init; } = Array.Empty<ClientBarberShopListItemDto>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public bool HasNextPage { get; init; }
}
