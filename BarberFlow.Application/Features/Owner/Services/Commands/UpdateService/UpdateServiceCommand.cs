using MediatR;

namespace BarberFlow.Application.Features.Services.Commands.UpdateService
{
    public class UpdateServiceCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        public int DisplayOrder { get; set; }
    }
}
