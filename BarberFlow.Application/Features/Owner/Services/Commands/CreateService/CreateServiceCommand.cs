using MediatR;

namespace BarberFlow.Application.Features.Services.Commands.CreateService;
public class CreateServiceCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
    public int DisplayOrder { get; set; }
}
