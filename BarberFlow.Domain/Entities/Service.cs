namespace BarberFlow.Domain.Entities
{
    public class Service
    {
        public Guid Id { get; private set; }
        public Guid BarberShopId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public BarberShop BarberShop { get; private set; }
        public ICollection<ServicePrice> ServicePrices { get; private set; }
        public ICollection<AppointmentService> AppointmentServices { get; private set; }

        private Service()
        {
            ServicePrices = new List<ServicePrice>();
            AppointmentServices = new List<AppointmentService>();
        }

        public Service(Guid barberShopId, string name, string description)
        {
            Id = Guid.NewGuid();
            BarberShopId = barberShopId;
            Name = name;
            Description = description;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            ServicePrices = new List<ServicePrice>();
            AppointmentServices = new List<AppointmentService>();
        }
    }
}
