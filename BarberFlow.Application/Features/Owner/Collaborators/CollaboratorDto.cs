namespace BarberFlow.Application.Features.Collaborators;
public class CollaboratorDto
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
