using MediatR;

namespace BarberFlow.Application.Features.Services.Queries.GetMyServices;
public class GetMyServicesQuery : IRequest<List<ServiceDto>>
{
}
