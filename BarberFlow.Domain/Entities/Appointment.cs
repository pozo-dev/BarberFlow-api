using BarberFlow.Domain.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BarberFlow.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid BarberShopId { get; private set; }
        public DateTime StartDateTime { get; private set; }
        public DateTime EndDateTime { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public User User { get; private set; }
        public BarberShop BarberShop { get; private set; }
        public ICollection<AppointmentService> AppointmentServices { get; private set; }

        private Appointment()
        {
            AppointmentServices = new List<AppointmentService>();
        }

        public Appointment(Guid userId, Guid barberShopId, DateTime start, DateTime end)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            BarberShopId = barberShopId;
            StartDateTime = start;
            EndDateTime = end;
            Status = AppointmentStatus.Scheduled;
            CreatedAt = DateTime.UtcNow;
            AppointmentServices = new List<AppointmentService>();
        }

        public void Completed() => Status = AppointmentStatus.Completed;
        public void Cancel() => Status = AppointmentStatus.Cancelled;
        public void NoShow() => Status = AppointmentStatus.NoShow;

        public bool CanBeCancelled()
        {
            var isInFuture = StartDateTime > DateTime.UtcNow;
            return Status == AppointmentStatus.Scheduled && isInFuture;
        }

        public void AddServices(List<Service> services, Dictionary<Guid, decimal> prices)
        {
            foreach (var service in services)
            {
                var price = prices[service.Id];

                var appointmentService = new AppointmentService(Id, service.Id, price);

                AppointmentServices.Add(appointmentService);
            }
        }
    }
}
