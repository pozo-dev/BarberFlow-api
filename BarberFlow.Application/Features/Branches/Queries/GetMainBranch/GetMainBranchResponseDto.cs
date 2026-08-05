namespace BarberFlow.Application.Features.Branches.Queries.GetMainBranch
{
    public class GetMainBranchResponseDto
    {
        public Guid BranchId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }

        public bool IsActive { get; set; }
    }
}
