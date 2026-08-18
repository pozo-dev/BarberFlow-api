using BarberFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace BarberFlow.Application.Features.Client.Appointments.Queries.GetAvailableProfessionals;

public sealed record GetAvailableProfessionalsQuery(Guid BranchId) : IRequest<IReadOnlyList<ClientProfessionalDto>>;

public sealed class GetAvailableProfessionalsHandler : IRequestHandler<GetAvailableProfessionalsQuery, IReadOnlyList<ClientProfessionalDto>>
{
    private readonly IBranchRepository _branches;
    private readonly ICollaboratorRepository _collaborators;

    public GetAvailableProfessionalsHandler(IBranchRepository branches, ICollaboratorRepository collaborators)
    {
        _branches = branches;
        _collaborators = collaborators;
    }

    public async Task<IReadOnlyList<ClientProfessionalDto>> Handle(GetAvailableProfessionalsQuery request, CancellationToken cancellationToken)
    {
        var branch = await _branches.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch is null || !branch.IsActive) return Array.Empty<ClientProfessionalDto>();
        var collaborators = await _collaborators.GetActiveByBranchIdAsync(branch.Id, cancellationToken);
        return collaborators
            .Select(x => new ClientProfessionalDto { Id = x.Id, Name = x.FullName })
            .ToList();
    }
}

public sealed class ClientProfessionalDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
