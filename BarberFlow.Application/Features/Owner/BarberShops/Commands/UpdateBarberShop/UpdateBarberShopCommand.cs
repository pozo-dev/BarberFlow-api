using MediatR;

namespace BarberFlow.Application.Features.BarberShops.Commands.UpdateBarberShop;
public class UpdateBarberShopCommand
    : IRequest<UpdateBarberShopResponseDto>
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public byte[] Logo { get; set; } = Array.Empty<byte>();

    public byte[] Banner { get; set; } = Array.Empty<byte>();
}
