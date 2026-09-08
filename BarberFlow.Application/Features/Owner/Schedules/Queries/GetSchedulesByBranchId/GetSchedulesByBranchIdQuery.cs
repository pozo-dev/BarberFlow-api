using MediatR;

namespace BarberFlow.Application.Features.Schedules.Queries.GetSchedulesByBranchId;
public class GetSchedulesByBranchIdQuery : IRequest<List<ScheduleDto>>
{
    public Guid BranchId { get; set; }
}
