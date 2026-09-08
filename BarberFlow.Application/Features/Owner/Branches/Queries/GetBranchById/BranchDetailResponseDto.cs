namespace BarberFlow.Application.Features.Branches.Queries.GetBranchById;
public class BranchDetailResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public int LocationSearchId { get; set; }
    public string LocationDisplayName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsMain { get; set; }

    public bool IsActive { get; set; }

}
