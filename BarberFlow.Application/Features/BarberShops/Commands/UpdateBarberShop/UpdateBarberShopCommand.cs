using MediatR;

namespace BarberFlow.Application.Features.BarberShops.Commands.UpdateBarberShop
{
    public class UpdateBarberShopCommand
        : IRequest<UpdateBarberShopResponseDto>
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}