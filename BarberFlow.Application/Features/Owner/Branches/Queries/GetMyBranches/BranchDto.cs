namespace BarberFlow.Application.Features.Branches.Queries.GetMyBranches
{
    public class BranchDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public bool IsMain { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
