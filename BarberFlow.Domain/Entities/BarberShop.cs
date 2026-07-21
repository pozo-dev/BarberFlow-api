namespace BarberFlow.Domain.Entities
{
    public class BarberShop
    {
        public Guid Id { get; private set; }

        public Guid OwnerUserId { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string PhoneNumber { get; private set; }

        public string Address { get; private set; }

        public string City { get; private set; }

        public TimeOnly OpenTime { get; private set; }

        public TimeOnly CloseTime { get; private set; }

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

        public BarberShop(
            Guid ownerUserId,
            string name,
            string description,
            string phoneNumber,
            string address,
            string city,
            TimeOnly openTime,
            TimeOnly closeTime)
        {
            Id = Guid.NewGuid();
            OwnerUserId = ownerUserId;
            Name = name;
            Description = description;
            PhoneNumber = phoneNumber;
            Address = address;
            City = city;
            OpenTime = openTime;
            CloseTime = closeTime;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;

            Services = new List<Service>();
            Barbers = new List<UserProfile>();
            Appointments = new List<Appointment>();
            Branches = new List<Branch>();
        }

        public void Update(
            string name,
            string description,
            string phoneNumber,
            string address,
            string city,
            TimeOnly openTime,
            TimeOnly closeTime)
        {
            Name = name;
            Description = description;
            PhoneNumber = phoneNumber;
            Address = address;
            City = city;
            OpenTime = openTime;
            CloseTime = closeTime;
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