using MediatR;

namespace BarberFlow.Application.Features.Schedules.Commands.UpdateSchedules;
public class UpdateSchedulesCommand
    : IRequest<UpdateSchedulesResponseDto>
{
    public Guid BranchId { get; set; }

    public List<UpdateScheduleItemCommand> Schedules { get; set; } = [];
}
