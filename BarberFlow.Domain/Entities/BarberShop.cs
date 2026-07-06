namespace BarberFlow.Domain.Entities
{
    public class BarberShop
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid OwnerUserId { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public ICollection<Service> Services { get; private set; }
        public ICollection<UserProfile> Barbers { get; private set; }
        public ICollection<Appointment> Appointments { get; private set; }

        private BarberShop()
        {
            Services = new List<Service>();
            Barbers = new List<UserProfile>();
            Appointments = new List<Appointment>();
        }

        public BarberShop(string name, Guid ownerUserId, string description)
        {
            Id = Guid.NewGuid();
            Name = name;
            OwnerUserId = ownerUserId;
            Description = description;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            Services = new List<Service>();
            Barbers = new List<UserProfile>();
            Appointments = new List<Appointment>();
        }
    }
}
