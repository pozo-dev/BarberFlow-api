using MediatR;

namespace BarberFlow.Application.Features.Branches.Commands.UpdateBranch
{
    public class UpdateBranchCommand
        : IRequest<UpdateBranchResponseDto>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int LocationSearchId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

    }
}
