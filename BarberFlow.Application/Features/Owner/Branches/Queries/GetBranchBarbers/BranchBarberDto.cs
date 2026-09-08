namespace BarberFlow.Application.Features.Branches.Queries.GetBranchBarbers;
public class BranchBarberDto
{
    public Guid BarberProfileId { get; set; }

    public Guid UserId { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }
}
