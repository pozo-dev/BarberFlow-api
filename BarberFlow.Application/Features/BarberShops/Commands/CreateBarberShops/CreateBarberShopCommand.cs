using MediatR;

namespace BarberFlow.Application.Features.BarberShops.Commands.CreateBarberShop
{
    public class CreateBarberShopCommand
        : IRequest<CreateBarberShopResponseDto>
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Datos de la sucursal principal
        public string PhoneNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

    }
}
