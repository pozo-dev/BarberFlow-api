namespace BarberFlow.Domain.Entities
{
    public class BarberShop
    {
        public Guid Id { get; private set; }

        public Guid OwnerUserId { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public ICollection<Service> Services { get; private set; }

        public ICollection<UserProfile> Barbers { get; private set; }

        public ICollection<Appointment> Appointments { get; private set; }

        public ICollection<Branch> Branches { get; private set; }

        private BarberShop()
        {
            Services = new List<Service>();
            Barbers = new List<UserProfile>();
            Appointments = new List<Appointment>();
            Branches = new List<Branch>();
        }

        private BarberShop(
            Guid ownerUserId,
            string name,
            string description)
        {
            Id = Guid.NewGuid();

            OwnerUserId = ownerUserId;

            Name = name.Trim();

            Description = description.Trim();

            IsActive = true;

            CreatedAt = DateTime.UtcNow;

            Services = new List<Service>();
            Barbers = new List<UserProfile>();
            Appointments = new List<Appointment>();
            Branches = new List<Branch>();
        }

        public static BarberShop Create(
            Guid ownerUserId,
            string name,
            string description)
        {
            return new BarberShop(ownerUserId, name, description);
        }

        public void Update(
            string name,
            string description)
        {
            Name = name.Trim();

            Description = description.Trim();
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}