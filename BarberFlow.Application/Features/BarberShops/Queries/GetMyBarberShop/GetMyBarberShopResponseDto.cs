namespace BarberFlow.Application.Features.BarberShops.Queries.GetMyBarberShop
{
    public class GetMyBarberShopResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        //public string PhoneNumber { get; set; } = string.Empty;

        //public string Address { get; set; } = string.Empty;

        //public string City { get; set; } = string.Empty;

        //public TimeOnly OpenTime { get; set; }

        //public TimeOnly CloseTime { get; set; }
    }
}
