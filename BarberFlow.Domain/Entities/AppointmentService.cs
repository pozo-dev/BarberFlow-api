namespace BarberFlow.Domain.Entities
{
    public class AppointmentService
    {
        public Guid AppointmentId { get; private set; }
        public Guid ServiceId { get; private set; }
        public decimal PriceAtTheMoment { get; private set; }
        public Appointment Appointment { get; private set; }
        public Service Service { get; private set; }

        private AppointmentService() { }

        public AppointmentService(Guid appointmentId, Guid serviceId, decimal price)
        {
            AppointmentId = appointmentId;
            ServiceId = serviceId;
            PriceAtTheMoment = price;
        }
    }
}
