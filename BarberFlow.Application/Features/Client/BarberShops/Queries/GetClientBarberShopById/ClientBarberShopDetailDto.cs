namespace BarberFlow.Application.Features.Client.BarberShops.Queries.GetClientBarberShopById;

public sealed class ClientBarberShopDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Logo { get; init; }
    public string? Banner { get; init; }
    public IReadOnlyList<ClientBranchDto> Branches { get; init; } = Array.Empty<ClientBranchDto>();
}

public sealed class ClientBranchDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public int LocationSearchId { get; init; }
    public string LocationDisplayName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public bool IsMain { get; init; }
    public IReadOnlyList<ClientScheduleDto> Schedules { get; init; } = Array.Empty<ClientScheduleDto>();
}

public sealed class ClientScheduleDto
{
    public int DayOfWeek { get; init; }
    public TimeOnly OpenTime { get; init; }
    public TimeOnly CloseTime { get; init; }
    public bool IsClosed { get; init; }
}
